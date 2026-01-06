using Il2Cpp;
using Il2CppSystem;
using Il2CppSystem.Collections;
using Il2CppSystem.Collections.Generic;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Delegate = Il2CppSystem.Delegate;

namespace RuinedMending
{
	internal class RMUtils
	{
		//Reference for the most recently selected item
		internal static GearItem restoreItem;

		//Returns an array of GearItems required to restore an item
		public static GearItem[] LookupRequiredRestoreGear(GearItem gi)
		{
			return gi.GetComponent<Repairable>().m_RequiredGear;
		}

		//Returns an array of ints for how many GearItems are required to restore an item
		public static int[] LookupRequiredRestoreGearAmounts(GearItem gi)
		{
			int[] requiredAmount = gi.GetComponent<Repairable>().m_RequiredGearUnits;

			for (int i = 0; i < requiredAmount.Length; i++)
			{
				requiredAmount[i] *= Settings.options.restoreMaterialMultiplier;
			}

			return requiredAmount;
		}

		//Returns how long the restore should take
		public static float LookupRestoreDuration(GearItem gi)
		{
			return gi.m_Repairable.m_DurationMinutes * GameManager.GetSkillClothingRepair().GetRepairTimeScale() * (IsUsingFishingTackle() ? 2f : 1f) * Settings.options.restoreDurationMultiplier;
		}

		//Rolls restore chance. Takes the base chance, adds the 20% boost, and then subtracts the debuff from the settings. Generates a random number based on that and clamps it between 0.2 and 1.0.
		public static float RollRestoreChance(GearItem gi)
		{
			float chanceToSucceed = (GameManager.GetSkillClothingRepair().GetBaseChanceSuccess() + 20 - Settings.options.restoreFailureDebuff) / 100;
			return System.Math.Clamp(UnityEngine.Random.RandomRange(chanceToSucceed, 1 + chanceToSucceed), 0.2f, 1f);
		}

		//Checks if the GearItem is a valid restore target
		public static bool IsValidItem(GearItem gi)
		{
			if (gi == null) return false; //GearItem is null

			if (!gi.m_ClothingItem) return false; //GearItem is not a clothing item

			if (!gi.m_WornOut || gi.m_CurrentHP != 0) return false; //GearItem isn't worn out and it's HP isn't 0

			return true; //GearItem is valid to restore
		}

		//Checks if player can restore the GearItem. Warns the player if they can't and "silent" is false.
		internal static bool CanRestore(GearItem gi, bool silent = true)
		{
			if (gi == null) return false; //GearItem is null

			//TODO Add light check here

			//Check if the player has all the required restore materials in their inventory.
			GearItem[] requiredMaterials = LookupRequiredRestoreGear(gi);
			int[] numRequiredMaterials = LookupRequiredRestoreGearAmounts(gi);
			for (int i = 0; i < requiredMaterials.Length; i++) 
			{
				if (GameManager.GetInventoryComponent().NumGearInInventory(requiredMaterials[i].name, true) < numRequiredMaterials[i])
				{
					//Enable HintLabel telling player what materials they need to restore the GearItem
					if (!silent) RMButtons.UpdateHintLabel(GetTextFriendlyMissingMaterialsString(gi), true);

					return false;
				}
			}

			if (GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit", true) == 0 && GameManager.GetInventoryComponent().NumGearInInventory("GEAR_HookAndLine", true) == 0)
			{
				//Enable HintLabel telling player what tools they need to restore the GearItem
				if (!silent) RMButtons.UpdateHintLabel("You need a Sewing Kit or a Fishing Tackle to repair this!", true);

				return false;
			}

			return true;
		}

		//Checks if a player is using a Fishing Tackle to restore an item
		internal static bool IsUsingFishingTackle()
		{
			return (GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit", true) == 0);
		}

		//Returns a nicely formatted string of required repair materials
		internal static string GetTextFriendlyMissingMaterialsString(GearItem gi)
		{
			string s_requiredMaterials = "";

			GearItem[] requiredMaterials = LookupRequiredRestoreGear(gi);
			int[] numRequiredMaterials = LookupRequiredRestoreGearAmounts(gi);

			if (requiredMaterials.Length == 0)
			{
				//What?
				RuinedMending.Log("GetTextFriendlyMissingMaterialsString() : Required restore materials is empty.", ComplexLogger.FlaggedLoggingLevel.Error);
			}
			else if (requiredMaterials.Length == 1)
			{
				s_requiredMaterials = $"{gi.DisplayName} requires {numRequiredMaterials[0]} {requiredMaterials[0].DisplayName} to repair!";
			} 
			else if (requiredMaterials.Length == 2)
			{
				s_requiredMaterials = $"{gi.DisplayName} requires {numRequiredMaterials[0]} {requiredMaterials[0].DisplayName} and {numRequiredMaterials[1]} {requiredMaterials[1].DisplayName} to repair!";
			}
			else
			{
				s_requiredMaterials = $"{gi.DisplayName} requires ";

				for (int i = 0; i < requiredMaterials.Length - 1; i++)
				{
					s_requiredMaterials += $"{numRequiredMaterials[i]} {requiredMaterials[i].DisplayName},";
				}

				s_requiredMaterials += $", and {numRequiredMaterials[1]} {requiredMaterials[1].DisplayName} to repair!";
			}

			return s_requiredMaterials;
		}

		internal class LabelCoroutine : MonoBehaviour
		{
			internal System.Collections.IEnumerator DisplayHintLabel(float waitTime)
			{
				//TODO Add nice fade out for HintLabel

				yield return new WaitForSeconds(waitTime);

				RMButtons.UpdateHintLabel("",false);
			}
		}
	}
}
