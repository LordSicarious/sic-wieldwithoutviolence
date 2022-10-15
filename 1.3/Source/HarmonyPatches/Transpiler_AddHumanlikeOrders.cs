//Transpiler for [RimWorld.FloatMenuMakerMap.AddHumanlikeOrders]
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace WieldWithoutViolence {
public static class Transpiler_AddHumanlikeOrders {
    // Skips the check to see if a pawn is incapable of violence when generating the floatmenu options to equip it
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        List<CodeInstruction> codes = new List<CodeInstruction>(instructions);
        int i, target = -1, start = -1, end = -1;
        bool found = false;
        for (i = 0; i < codes.Count; i++) {
            // Look for the "CannotEquip" and "IsIncapableOfViolenceLower" strings
            if (codes[i].opcode == OpCodes.Ldstr) {
                if (codes[i].operand as string == "CannotEquip") {
                    target = i;
                } else if (codes[i].operand as string == "IsIncapableOfViolenceLower") {
                    if (i - target < 10) { found = true; break; }
                }
            }
        }
        if (found) {
            // Scroll back from target to find if statement
            for (i = target; i >= 0; i--) {
                if (codes[i].opcode == OpCodes.Brfalse_S && end < 0) {
                    end = i;
                }
                if (codes[i].opcode == OpCodes.Stloc_S) {
                    start = i+1;
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