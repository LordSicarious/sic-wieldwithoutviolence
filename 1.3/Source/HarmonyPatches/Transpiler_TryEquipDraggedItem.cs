//Transpiler for [RimWorld.WITab_Caravan_Gear.TryEquipDraggedItem]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace WieldWithoutViolence {
public static class Transpiler_TryEquipDraggedItem {
    // Skips the check to see if a pawn is incapable of violence when generating the floatmenu options to equip it
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        int i, target = -1, start = -1, end = -1;
        for (i = 0; i < codes.Count; i++) {
            // Look for target string
            if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand as string == "MessageCantEquipIncapableOfViolence") {
                target = i;
                break;
            }
        }
        if (target >= 0) {
            // Scroll back from target to find if statement
            for (i = target; i >= 0; i--) {
                if (codes[i].opcode == OpCodes.Brfalse_S) {
                    end = i;
                }
                if (codes[i].opcode == OpCodes.Ldloc_0) {
                    start = i;
                    break;
                }
            }
        }
        if (start < 0 || end < 0) { Log.Error("Cannot find transpiler target."); }
        else {
            Log.Message("Patching between " + codes[start].ToString() + " and " + codes[end].ToString() + ", length: " + (1+ end - start).ToString());
            codes[start].opcode = OpCodes.Ldc_I4_0;
            codes[start].operand = null;
            codes.RemoveRange(start + 1, end - start - 1);
        }
        return codes;
    }
}
}