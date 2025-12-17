using Il2Cpp;
using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Delegate = Il2CppSystem.Delegate;

namespace RuinedMending
{
	internal class RMUtils
	{
		//Item to restore placeholder
		internal static GearItem restoreItem;

		public static string LookupRestoreMaterial(GearItem gi)
		{
			//TODO Add logic to decide which material to repair clothes with
			return "GEAR_Cloth";
		}

		public static int LookupRestoreAmount(GearItem gi)
		{
			//TODO Add logic to decide which material to repair clothes with
			return 2;
		}

		public static bool IsValidItem(GearItem gi)
		{
			if (gi == null) return false; //GearItem is null

			if (!gi.m_ClothingItem) return false; //GearItem is not a clothing item

			if (!gi.m_WornOut || gi.m_CurrentHP != 0) return false; //GearItem isn't worn out and it's HP isn't 0

			return true; //GearItem is valid to restore
		}

		internal static bool CanRestore(GearItem gi)
		{
			if (gi == null) return false; //GearItem is null

			int numRestoreMaterials = GameManager.GetInventoryComponent().NumGearInInventory(LookupRestoreMaterial(gi));

			RuinedMending.Log($"Number of restore materials: {numRestoreMaterials}. Sewing Kits: {GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit")}. Tackles: {GameManager.GetInventoryComponent().NumGearInInventory("GEAR_HookAndLine")}.");

			if (numRestoreMaterials < LookupRestoreAmount(gi)) return false; //TODO Add warning message to player here for lack of materials

			if (GameManager.GetInventoryComponent().NumGearInInventory("GEAR_SewingKit") == 0 && GameManager.GetInventoryComponent().NumGearInInventory("GEAR_HookAndLine") == 0) return false; //TODO Add warning to player here for lack of tools


			return true;
		}
	}
}
