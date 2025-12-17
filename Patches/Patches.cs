using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using Il2Cpp;
using Il2CppNewtonsoft.Json.Linq;
using HarmonyLib;


namespace RuinedMending
{
	//RuinedMending.Log($"1.CanRepair: {__instance.CanRepair()}. 2.HasMaterial: {__instance.RepairHasRequiredMaterial()}. 3.HasTool: {__instance.RepairHasRequiredTool()}. 4.Duration Minutes: {__instance.m_Repairable.GetDurationMinutes()}. 5.WillSucceed: {__instance.m_RepairWillSucceed}. 6.Material Required: {__instance.m_Repairable.GetMaterialRequired(1)}. 7.DisplayName: {__instance.m_Repairable.GetDisplayName()}");


	//Lets us grab a copy of an existing button to copy later.
	[HarmonyPatch(typeof(Panel_Inventory), nameof(Panel_Inventory.Initialize))]
	internal class RuinedMendingInitialization
	{
		private static void Postfix(Panel_Inventory __instance)
		{
			RuinedMending.Log("Initializing RM panel");
			RMButtons.InitializeRM(__instance.m_ItemDescriptionPage);
		}
	}

	//Updates selected GearItem so we know which item we are restoring.
	[HarmonyPatch(typeof(ItemDescriptionPage), nameof(ItemDescriptionPage.UpdateGearItemDescription))]
	internal class UpdateRestoreItemButton
	{
		private static void Postfix(ItemDescriptionPage __instance, GearItem gi)
		{
			if (__instance != InterfaceManager.GetPanel<Panel_Inventory>()?.m_ItemDescriptionPage) return;
			
			if (RMUtils.IsValidItem(gi))
			{
				RMButtons.SetRestoreItemButtonActive(true);

				if (RMUtils.CanRestore(gi))
				{
					RuinedMending.Log($"'{gi.name}' is valid to repair");

					RMButtons.SetRestoreItemButtonErrored(false);
					RMUtils.restoreItem = gi;
				} else
				{
					RMButtons.SetRestoreItemButtonErrored(true);
				}
			}
			else
			{
				RMButtons.SetRestoreItemButtonActive(false);
			}
		}
	}
}


