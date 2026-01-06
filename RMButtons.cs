using Il2Cpp;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuinedMending
{
	internal class RMButtons
	{
		//Buttons
		private static GameObject restoreButton;

		internal static UILabel hintLabel;

		//Button Text Localization
		internal static string restoreText = "Restore";

		internal static void InitializeRM(ItemDescriptionPage itemDescriptionPage)
		{
			//restoreText = Localization.Get("GAMEPLAY_RM_RestoreButtonLabel"); TODO Add localization

			//Creates a button labeled Restore
			GameObject equipButton = itemDescriptionPage.m_MouseButtonEquip;
			restoreButton = UnityEngine.Object.Instantiate<GameObject>(equipButton, equipButton.transform.parent, true);
			Utils.GetComponentInChildren<UILabel>(restoreButton).text = restoreText;
			SetAction(restoreButton, new System.Action(OnRestoreItem));

			//Creates a HintLabel to display information
			UILabel itemLabel = itemDescriptionPage.m_ItemDescLabel;
			hintLabel = UnityEngine.Object.Instantiate<UILabel>(itemLabel, itemLabel.transform.parent, true);
			hintLabel.text = "";
			hintLabel.transform.Translate(0, -1.35f, 0);
			hintLabel.color = Color.red;
			hintLabel.enabled = false;
		}

		//Sets a method to run when a button is clicked
		private static void SetAction(GameObject button, System.Action action)
		{
			Il2CppSystem.Collections.Generic.List<EventDelegate> placeHolderList = new Il2CppSystem.Collections.Generic.List<EventDelegate>();
			placeHolderList.Add(new EventDelegate(action));
			Utils.GetComponentInChildren<UIButton>(button).onClick = placeHolderList;
		}

		//Action called when user clicked Restore button
		private static void OnRestoreItem()
		{
			if (!RMUtils.CanRestore(RMUtils.restoreItem, false))
			{
				GameAudioManager.PlaySound(GameAudioManager.Instance.m_ErrorAudio, GameManager.m_PlayerObject); //Only way I could get Audio to play. Does GameAudioManager.Play not work anymore?
				return;
			}

			GameAudioManager.PlaySound(GameAudioManager.Instance.m_Confirm, GameManager.m_PlayerObject);

			float restoreTime = RMUtils.LookupRestoreDuration(RMUtils.restoreItem);
			float failureThreshold = RMUtils.RollRestoreChance(RMUtils.restoreItem);

			//TODO Add localization here
			InterfaceManager.GetPanel<Panel_GenericProgressBar>().Launch("Restoring", 5f, restoreTime, failureThreshold,
							"Play_CraftingCloth", null, false, true, new System.Action<bool, bool, float>(OnRestoreItemFinished));
		}

		//Method called when restoring action has cancelled, failed, or finished.
		private static void OnRestoreItemFinished(bool success, bool playerCancel, float progress)
		{
			//Player cancelled. Don't consume materials or degrade tool.
			if (playerCancel) return;

			//Get the tool used and degrade it
			string toolUsed = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit", true) > 0 ? "GEAR_SewingKit" : "GEAR_HookAndLine";
			GearItem gi = GameManager.GetInventoryComponent().GetLowestConditionGearThatMatchesName(toolUsed);
			gi.DegradeOnUse();

			//Get required restore materials/amounts and remove them from players inventory.
			GearItem[] requiredMaterials = RMUtils.LookupRequiredRestoreGear(RMUtils.restoreItem);
			int[] numRequiredMaterials = RMUtils.LookupRequiredRestoreGearAmounts(RMUtils.restoreItem);
			for (int i = 0; i < requiredMaterials.Length; i++)
			{
				GameManager.GetInventoryComponent().RemoveGearFromInventory(requiredMaterials[i].name, numRequiredMaterials[i]);
			}

			//If repair was successful, restore item.
			if (success)
			{
				RMUtils.restoreItem.SetNormalizedHP((Settings.options.restoredItemCondition / 100) - 0.01f, true);
				RMUtils.restoreItem.ForceNotWornOut();
			}
			else
			{
				GameAudioManager.PlaySound(GameAudioManager.Instance.m_ErrorAudio, GameManager.m_PlayerObject);
				UpdateHintLabel($"Restoring Failed. {(GameManager.GetSkillClothingRepair().GetBaseChanceSuccess() + 20 - Settings.options.restoreFailureDebuff).ToString("0.##")}% chance of success.");
			}
		}

		//Enables or disables the Restore button
		internal static void SetRestoreItemButtonActive(bool active)
		{
			NGUITools.SetActive(restoreButton, active);
		}

		//TODO Fix error - Button defaulting to red after mouse over.
		internal static void SetRestoreItemButtonErrored(bool errored)
		{
			//if (errored)
			//{
			//	Utils.GetComponentInChildren<UILabel>(restoreButton).color = Color.red;
			//}
			//else
			//{
			//	Utils.GetComponentInChildren<UILabel>(restoreButton).color = Color.gray;
			//}
		}

		//Updates HintLabel text and enables or disabled it.
		internal static void UpdateHintLabel(string text, bool enabled = true)
		{
			hintLabel.text = text;
			hintLabel.enabled = enabled;

			if (enabled)
			{
				var coroutine = new RMUtils.LabelCoroutine();

				MelonCoroutines.Start(coroutine.DisplayHintLabel(5f));
			}
		}

	}
}
