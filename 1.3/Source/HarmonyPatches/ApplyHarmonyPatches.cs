using System;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace WieldWithoutViolence {
    [StaticConstructorOnStartup]
    public static class ApplyHarmonyPatches {
		// Reference to this class for patches
        static ApplyHarmonyPatches() {
			// Instantiate Harmony
			var harmony = new Harmony("sic.WieldWithoutViolence.thisisanid");
			Type patchType;
			MethodInfo original;
			string modified;

			//Transpiler for [RimWorld.FloatMenuMakerMap.AddHumanlikeOrders]
            patchType = typeof(Transpiler_AddHumanlikeOrders);
            original = AccessTools.Method(typeof(FloatMenuMakerMap), name: "AddHumanlikeOrders");
            modified = nameof(Transpiler_AddHumanlikeOrders.Transpiler);
            harmony.Patch(original, transpiler: new HarmonyMethod(patchType, modified));

			//Transpiler for [RimWorld.WITab_Caravan_Gear.TryEquipDraggedItem]
            patchType = typeof(Transpiler_TryEquipDraggedItem);
            original = AccessTools.Method(typeof(WITab_Caravan_Gear), name: "TryEquipDraggedItem");
            modified = nameof(Transpiler_TryEquipDraggedItem.Transpiler);
            harmony.Patch(original, transpiler: new HarmonyMethod(patchType, modified));
        }
    }
}