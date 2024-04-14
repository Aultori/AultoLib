namespace AultoLib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection.Emit;
    using HarmonyLib;
    using RimWorld;
    using Verse;
    using System.Reflection;
    using static HarmonyLib.Code;
    using AultoLib.CustomProperties;
    using UnityEngine;

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            AultoLog.Message("Starting patching");

            // var harmony = new Harmony("AultoLib.patcher");
            var harmony = new Harmony(id: "AultoLib.patcher");

            // harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith)),
            //     prefix: new HarmonyMethod(patchType, nameof(TestPrefix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith)),
                transpiler: new HarmonyMethod(patchType, nameof(TryInteractWithTranspiler)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.InteractionsTrackerTick)),
                transpiler: new HarmonyMethod(patchType, nameof(InteractionsTrackerTickTranspiler)));

            harmony.Patch(AccessTools.Method(typeof(Bubbles.Core.Bubbler), nameof(Bubbles.Core.Bubbler.Add)), transpiler: new HarmonyMethod(patchType, nameof(Bubbles_Add_Transpiler)));
            harmony.Patch(AccessTools.Method(typeof(Bubbles.Core.Bubble), "GetText"), transpiler: new HarmonyMethod(patchType, nameof(Bubbles_ReplaceEntryWithInteractionInstance_Transpiler)));
            harmony.Patch(AccessTools.Method(typeof(Bubbles.Core.Bubble), "GetFade"), transpiler: new HarmonyMethod(patchType, nameof(Bubbles_ReplaceEntryWithInteractionInstance_Transpiler)));


        }


        // public static void InteractionsTrackerTickTranspiler()
        // {

        // }
        public static IEnumerable<CodeInstruction> TryInteractWithTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo  pawn = AccessTools.Field(typeof(Pawn_InteractionsTracker), "pawn");
            MethodInfo test = AccessTools.Method(patchType, nameof(TestTranspiler));
            // yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(patchType, nameof(TestTranspiler)));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            //yield return CodeInstruction.LoadField(typeof(Pawn), "pawn");
            //yield return CodeInstruction.LoadField(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.pawn));
            yield return new CodeInstruction(OpCodes.Ldfld, pawn);
            yield return new CodeInstruction(OpCodes.Call, test);
            //yield return CodeInstruction.Call(patchType, nameof(TestTranspiler));

            foreach (CodeInstruction instruction in instructions)
                yield return instruction;
        }

        public static void TestPrefix()
        {
            AultoLog.Warning("Prefix worked!!");
        }

        public static void TestTranspiler(Pawn pawn)
        {
            AultoLog.Warning("Transpiler worked!!");
            AultoLog.Warning($"Pawn {pawn.Name} tried to interact");
        }

        public static IEnumerable<CodeInstruction> InteractionsTrackerTickTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // overriding the default functionality
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "pawn"));
            yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.LoadPawn));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "wantsRandomInteract"));
            yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.LoadWantsRandomInteract));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "lastInteractionTime"));
            yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.LoadLastInteractionTime));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "lastInteraction"));
            yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.LoadLastInteraction));
            // yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "pawn"));
            // yield return new CodeInstruction(OpCodes.Ldarg_0);
            // yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "wantsRandomInteract"));
            // yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "wantsRandomInteract"));
            // yield return new CodeInstruction(OpCodes.Ldarg_0);
            // yield return new CodeInstruction(OpCodes.Ldfld,  AccessTools.Field(typeof(Pawn_InteractionsTracker), "lastInteraction"));
            // yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "lastInteraction"));
            // yield return new CodeInstruction(OpCodes.Ldarg_0);
            // yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Pawn_InteractionsTracker), "get_CurrentSocialMode"));
            // yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "currentSocialMode"));
            yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AultoLib_Pawn_InteractionsTracker), "InteractionsTrackerTick"));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "wantsRandomInteract"));
            yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Pawn_InteractionsTracker), "wantsRandomInteract"));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "lastInteractionTime"));
            yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Pawn_InteractionsTracker), "lastInteractionTime"));
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "lastInteraction"));
            yield return new CodeInstruction(OpCodes.Stfld, AccessTools.Field(typeof(Pawn_InteractionsTracker), "lastInteraction"));
            yield return new CodeInstruction(OpCodes.Ret);

            foreach (CodeInstruction instruction in instructions)
                yield return instruction;
        }


        public static IEnumerable<CodeInstruction> Bubbles_Add_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
            LocalBuilder interactionInstanceLocal = ilg.DeclareLocal(typeof(AultoLib.PlayLogEntry_InteractionInstance));
            MethodInfo interactionInstance_GetInitiator = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetInitiator));
            MethodInfo interactionInstance_GetRecipient = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetRecipient));
            MethodInfo interactionInstance_GetLinkedInteraction = AccessTools.Method(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetLinkedInteraction));
            // MethodInfo interactionInstance_CreateLink = AccessTools.Method(typeof(LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.CreateLink));
            // MethodInfo interaction_GetInteractionInstance = AccessTools.Method(typeof(AultoLib.CustomProperties.LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.GetInteractionInstance));

            FieldInfo bubblesGetInitiator = AccessTools.Field(typeof(Bubbles.Access.Reflection), nameof(Bubbles.Access.Reflection.Verse_PlayLogEntry_Interaction_Initiator));
            FieldInfo bubblesGetRecipient = AccessTools.Field(typeof(Bubbles.Access.Reflection), nameof(Bubbles.Access.Reflection.Verse_PlayLogEntry_Interaction_Recipient));

            Label gotoRet = ilg.DefineLabel();
            Label skipRet = ilg.DefineLabel();

            // if (AultoLog.DoLog()) AultoLog.Message("Patching Bubbles Add");

            List<CodeInstruction> instructionList = instructions.ToList();
            for (int i = 0; i < instructionList.Count; i++)
            {
                if (instructionList[i].Is(OpCodes.Call, AccessTools.Method(typeof(Bubbles.Core.Bubbler), "ShouldShow")))
                {
                    AultoLog.Message("debug1");
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
                    // if (AultoLog.DoLog()) yield return new CodeInstruction(OpCodes.Ldstr, "Got here1");
                    // if (AultoLog.DoLog()) yield return CodeInstruction.Call(typeof(AultoLog), nameof(AultoLog.Message), new[] { typeof(string) });
                    yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Brtrue, skipRet);

                    yield return new CodeInstruction(OpCodes.Ret).WithLabels(gotoRet);

                    yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex).WithLabels(skipRet);
                    // if (AultoLog.DoLog()) yield return new CodeInstruction(OpCodes.Ldstr, "Made it past the return");
                    // if (AultoLog.DoLog()) yield return CodeInstruction.Call(typeof(AultoLog), nameof(AultoLog.Message), new[] { typeof(string) });
                    // yield return new CodeInstruction(OpCodes.Call, interactionInstance_CreateLink); // do this??, or whenever one is created, so the ticks are correct
                    yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetLinkedInteraction);
                    yield return new CodeInstruction(OpCodes.Stloc_0);

                    // if (AultoLog.DoLog()) yield return new CodeInstruction(OpCodes.Ldstr, "Got here3");
                    // if (AultoLog.DoLog()) yield return CodeInstruction.Call(typeof(AultoLog), nameof(AultoLog.Message), new[] { typeof(string) });

                    continue;
                }
                if (instructionList[i].Is(OpCodes.Ldsfld, bubblesGetInitiator))
                {
                    AultoLog.Message("debug2");
                    yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                    yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetInitiator);
                    // yield return CodeInstruction.Call(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetInitiator));
                    yield return new CodeInstruction(OpCodes.Stloc_1);
                    i += 4;
                    continue;
                }
                if (instructionList[i].Is(OpCodes.Ldsfld, bubblesGetRecipient))
                {
                    AultoLog.Message("debug3");
                    // yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldloc, interactionInstanceLocal.LocalIndex);
                    // yield return new CodeInstruction(OpCodes.Ldarg_0);
                    // yield return new CodeInstruction(OpCodes.Call, nameof(AultoLib.PlayLogEntry_InteractionInstance.GetRecipient));
                    // yield return CodeInstruction.Call(typeof(PlayLogEntry_InteractionInstance), nameof(AultoLib.PlayLogEntry_InteractionInstance.GetRecipient));
                    yield return new CodeInstruction(OpCodes.Call, interactionInstance_GetRecipient);
                    yield return new CodeInstruction(OpCodes.Stloc_2);
                    i += 4;
                    continue;
                }
                // if (instructionList[i].Is(OpCodes.Newobj, AccessTools.Constructor(typeof(Bubbles.Core.Bubble), new[] {typeof(Verse.Pawn), typeof(Verse.PlayLogEntry_Interaction)} )))
                // {
                //     yield return new CodeInstruction(OpCodes.Newobj, AccessTools.Constructor(typeof(Bubbles.Core.Bubble), new[] {typeof(Verse.Pawn), typeof(Verse.LogEntry)} ));
                //     continue;
                // }
                yield return instructionList[i];
            }
        }

        // used multiple times
        public static IEnumerable<CodeInstruction> Bubbles_ReplaceEntryWithInteractionInstance_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // if (AultoLog.DoLog()) AultoLog.Message("Doing Replace Entry");
            // LocalBuilder interactionInstanceLocal = ilg.DeclareLocal(typeof(AultoLib.PlayLogEntry_InteractionInstance));
            MethodInfo interaction_GetInteractionInstance = AccessTools.Method(typeof(AultoLib.CustomProperties.LinkedProperty_PlayLogEntryInteraction), nameof(LinkedProperty_PlayLogEntryInteraction.GetInteractionInstance));

            List<CodeInstruction> instructionList = instructions.ToList();
            for (int i = 0; i < instructionList.Count; i++)
            {
                // if (instructionList[i].opcode == OpCodes.Ldarg_0 && instructionList[i + 1].Is(OpCodes.Call, "get_Entry"))
                if (instructionList[i].Is(OpCodes.Call, AccessTools.Method(typeof(Bubbles.Core.Bubble), "get_Entry")))
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
