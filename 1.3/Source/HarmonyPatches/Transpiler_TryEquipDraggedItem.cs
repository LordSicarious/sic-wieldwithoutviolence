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
        int insertAt = -1;
        Label? jumpLabel = null;
        bool found = false;
        int i, CannotEquipIndex = -1;
        for (i = 0; i < codes.Count; i++) {
            // Look for the "MessageCantEquipIncapableOfViolence" string
            if (codes[i].opcode == OpCodes.Ldstr && codes[i].operand as string == "MessageCantEquipIncapableOfViolence") {
                CannotEquipIndex = i;
                found = true;
                break;
            }
        }
        if (found) {
            // Scroll back from CannotEquip to find start of segment
            for (i = CannotEquipIndex; i >= 0; i--) {
                // Grab jump label on the way
                if (codes[i].opcode == OpCodes.Brfalse_S) {
                    jumpLabel = codes[i].operand as Label?;
                }
                // Search for the instance load before the two breaks in the if statement
                if (codes[i].opcode == OpCodes.Ldloc_0) {
                    insertAt = i;
                    break;
                }
            }
        }
        if (insertAt < 0 || jumpLabel == null) { Log.Error("Cannot find transpiler target."); }
        else {
            codes.Insert(insertAt, new CodeInstruction(OpCodes.Br, jumpLabel));
        }
        return codes;
    }
}
}