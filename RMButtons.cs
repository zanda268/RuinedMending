using Il2Cpp;
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

		//Button Text Localization
		internal static string restoreText = "Restore";

		internal static void InitializeRM(ItemDescriptionPage itemDescriptionPage)
		{
			//restoreText = Localization.Get("GAMEPLAY_IG_SeedExtractLabel"); //TODO Add localization

			GameObject equipButton = itemDescriptionPage.m_MouseButtonEquip;
			restoreButton = UnityEngine.Object.Instantiate<GameObject>(equipButton, equipButton.transform.parent, true);
			//restoreButton.transform.Translate(0, 0, 0);
			Utils.GetComponentInChildren<UILabel>(restoreButton).text = restoreText;

			SetAction(restoreButton, new System.Action(OnRestoreItem));
		}

		//Sets a method to run when a button is clicked
		private static void SetAction(GameObject button, System.Action action)
		{
			Il2CppSystem.Collections.Generic.List<EventDelegate> placeHolderList = new Il2CppSystem.Collections.Generic.List<EventDelegate>();
			placeHolderList.Add(new EventDelegate(action));
			Utils.GetComponentInChildren<UIButton>(button).onClick = placeHolderList;
		}

		private static void OnRestoreItem()
		{
			if (!RMUtils.CanRestore(RMUtils.restoreItem))
			{
				RuinedMending.Log($"Unable to repair item.");
				GameAudioManager.PlayGUIError(); //TODO Not playing audio
				return;
			}

			GameAudioManager.PlayGuiConfirm(); //TODO Not playing audio

			//TODO Add settings to change time/failure chances
			//TODO Add check for which tool we are using and slow down time
			float restoreTime = RMUtils.restoreItem.m_Repairable.m_DurationMinutes;
			float failureChance = 0f;

			//TODO Add localization here
			InterfaceManager.GetPanel<Panel_GenericProgressBar>().Launch("Restoring", 5f, restoreTime, failureChance,
							"Play_CraftingCloth", null, false, true, new System.Action<bool, bool, float>(OnRestoreItemFinished));
		}

		private static void OnRestoreItemFinished(bool success, bool playerCancel, float progress)
		{
			//TODO Add logic to check if repair was successful and consume items.

			if (playerCancel) return;

			string toolUsed = GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit") > 0 ? "GEAR_SewingKit" : "GEAR_HookAndLine";

			GearItem gi = GameManager.GetInventoryComponent().GetLowestConditionGearThatMatchesName(toolUsed);
			gi.DegradeOnUse();

			GameManager.GetInventoryComponent().RemoveGearFromInventory(RMUtils.LookupRestoreMaterial(RMUtils.restoreItem), RMUtils.LookupRestoreAmount(RMUtils.restoreItem));

			if (success)
			{
				RMUtils.restoreItem.SetNormalizedHP(0.04f); //TODO Add setting for amount to repair too.
				RMUtils.restoreItem.ForceNotWornOut();
			}
		}

		internal static void SetRestoreItemButtonActive(bool active)
		{
			NGUITools.SetActive(restoreButton, active);
		}

		internal static void SetRestoreItemButtonErrored(bool errored)
		{
			//TODO Fix error button resetting to white on mouse over.
			if (errored)
			{
				Utils.GetComponentInChildren<UILabel>(restoreButton).color = Color.red;
			}
			else
			{
				Utils.GetComponentInChildren<UILabel>(restoreButton).color = Color.gray;
			}
		}

	}
}
