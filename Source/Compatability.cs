using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using AultoLib.CustomProperties;

namespace AultoLib
{
    public static class Compatability
    {
        private static Harmony harmony;

        public static void Init(Harmony harmony)
        {
            Compatability.harmony = harmony;
        }

        public static class InteractionBubblesPatches
        {
            private static readonly Type patchType = typeof(Compatability.InteractionBubblesPatches);
            public static readonly bool bubblesLoaded = ModsConfig.IsActive("jaxe.bubbles");

            public static void DoPatchingIfLoaded()
            {
                AultoLog.Message("entered bubbles patching");
                if (!bubblesLoaded)
                {
                    AultoLog.Message("bubbles is not loaded");
                    return;
                }
                //if (!Compatability.InteractionBubblesPatches.Loaded) return;
                AultoLog.Message("patching bubbles");

                _= harmony.Patch(AccessTools.Method("Bubbles.Core.Bubbler:Add"),    transpiler: new HarmonyMethod(patchType, nameof(Bubbles_Add_Transpiler)));
                _= harmony.Patch(AccessTools.Method("Bubbles.Core.Bubble:GetText"), transpiler: new HarmonyMethod(patchType, nameof(Bubbles_ReplaceEntryWithInteractionInstance_Transpiler)));
                _= harmony.Patch(AccessTools.Method("Bubbles.Core.Bubble:GetFade"), transpiler: new HarmonyMethod(patchType, nameof(Bubbles_ReplaceEntryWithInteractionInstance_Transpiler)));

            }

            private static IEnumerable<CodeInstruction> Bubbles_Add_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
            {
                Log.Message($"entered Bubbles_Add_Transpiler");
                LocalBuilder interactionInstanceLocal = ilg.DeclareLocal(typeof(AultoLib.PlayLogEntry_InteractionInstance));
                MethodInfo interactionInstance_GetInitiator = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetInitiator));
                MethodInfo interactionInstance_GetRecipient = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetRecipient));
                MethodInfo interactionInstance_GetLinkedInteraction = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetLinkedInteraction));
                // MethodInfo interactionInstance_CreateLink = AccessTools.Method(typeof(LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.CreateLink));
                // MethodInfo interaction_GetInteractionInstance = AccessTools.Method(typeof(AultoLib.CustomProperties.LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.GetInteractionInstance));

                // FieldInfo bubblesGetInitiator = AccessTools.Field(typeof(Bubbles.Access.Reflection), nameof(Bubbles.Access.Reflection.Verse_PlayLogEntry_Interaction_Initiator));
                // FieldInfo bubblesGetRecipient = AccessTools.Field(typeof(Bubbles.Access.Reflection), nameof(Bubbles.Access.Reflection.Verse_PlayLogEntry_Interaction_Recipient));
                FieldInfo bubblesGetInitiator = AccessTools.Field("Bubbles.Access.Reflection:Verse_PlayLogEntry_Interaction_Initiator");
                FieldInfo bubblesGetRecipient = AccessTools.Field("Bubbles.Access.Reflection:Verse_PlayLogEntry_Interaction_Recipient");

                Label gotoRet = ilg.DefineLabel();
                Label skipRet = ilg.DefineLabel();

                // if (AultoLog.DoLog()) AultoLog.Message("Patching Bubbles Add");

                List<CodeInstruction> instructionList = instructions.ToList();
                for (int i = 0; i < instructionList.Count; i++)
                {
                    //if (instructionList[i].Is(OpCodes.Call, AccessTools.Method(typeof(Bubbles.Core.Bubbler), "ShouldShow")))
                    if (instructionList[i].Is(OpCodes.Call, AccessTools.Method("Bubbles.Core.Bubbler:ShouldShow")))
                    {
                        Log.Message($"{AultoLog.AdvancedPrefix()} debug1");
                        yield return instructionList[i];
                        // IL_0005: brfalse.s   IL_0011
                        // IL_0007: ldarg.0
                        // IL_0008: isinst      ['Assembly-CSharp']Verse.PlayLogEntry_Interaction
                        // IL_000D: stloc.0
                        // IL_000E: ldloc.0
                        // IL_000F: brtrue.s    IL_0012
                        // IL_0011: ret
                        i += 7;
                        yield return new CodeInstruction(OpCodes.Brfalse, gotoRet);

                        // if (AultoLog.DoLog()) yield return new CodeInstruction(OpCodes.Ldstr, "entered Add");
                        // if (AultoLog.DoLog()) yield return CodeInstruction.Call(typeof(AultoLog), nameof(AultoLog.Message), new[] { typeof(string) });

                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Isinst, typeof(AultoLib.PlayLogEntry_InteractionInstance));
                        yield return new CodeInstruction(OpCodes.Stloc, interactionInstanceLocal.LocalIndex);
                        yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                        yield return new CodeInstruction(OpCodes.Brtrue, skipRet);

                        yield return new CodeInstruction(OpCodes.Ret).WithLabels(gotoRet);

                        yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex).WithLabels(skipRet);
                        yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetLinkedInteraction);
                        yield return new CodeInstruction(OpCodes.Stloc_0);


                        continue;
                    }
                    if (instructionList[i].Is(OpCodes.Ldsfld, bubblesGetInitiator))
                    {
                        Log.Message($"{AultoLog.AdvancedPrefix()} debug2");
                        yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                        yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetInitiator);
                        yield return new CodeInstruction(OpCodes.Stloc_1);
                        i += 4;
                        continue;
                    }
                    if (instructionList[i].Is(OpCodes.Ldsfld, bubblesGetRecipient))
                    {
                        Log.Message($"{AultoLog.AdvancedPrefix()} debug3");
                        yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                        yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetRecipient);
                        yield return new CodeInstruction(OpCodes.Stloc_2);
                        i += 4;
                        continue;
                    }
                    yield return instructionList[i];
                }
            }

            // used multiple times
            private static IEnumerable<CodeInstruction> Bubbles_ReplaceEntryWithInteractionInstance_Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                // if (AultoLog.DoLog()) AultoLog.Message("Doing Replace Entry");
                // LocalBuilder interactionInstanceLocal = ilg.DeclareLocal(typeof(AultoLib.PlayLogEntry_InteractionInstance));
                MethodInfo interaction_GetInteractionInstance = AccessTools.Method(typeof(AultoLib.CustomProperties.LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.GetInteractionInstance));

                List<CodeInstruction> instructionList = instructions.ToList();
                for (int i = 0; i < instructionList.Count; i++)
                {
                    // if (instructionList[i].opcode == OpCodes.Ldarg_0 && instructionList[i + 1].Is(OpCodes.Call, "get_Entry"))
                    //if (instructionList[i].Is(OpCodes.Call, AccessTools.Method(typeof(Bubbles.Core.Bubble), "get_Entry")))
                    if (instructionList[i].Is(OpCodes.Call, AccessTools.Method("Bubbles.Core.Bubble:get_Entry")))
                    // if (instructionList[i].opcode == OpCodes.Ldarg_0 && instructionList[i + 1].opcode == OpCodes.Call)
                    {
                        yield return instructionList[i];
                        yield return new CodeInstruction(OpCodes.Call, interaction_GetInteractionInstance);
                        continue;
                    }
                    yield return instructionList[i];
                }
            }

        }
    }
}
