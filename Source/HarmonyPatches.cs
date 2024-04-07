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

    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        private static readonly Type patchType = typeof(HarmonyPatches);

        static HarmonyPatches()
        {
            Logging.Message("Starting patching");

            // var harmony = new Harmony("AultoLib.patcher");
            var harmony = new Harmony(id: "AultoLib.patcher");

            // harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith)),
            //     prefix: new HarmonyMethod(patchType, nameof(TestPrefix)));
            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.TryInteractWith)),
                transpiler: new HarmonyMethod(patchType, nameof(TryInteractWithTranspiler)));

            harmony.Patch(AccessTools.Method(typeof(Pawn_InteractionsTracker), nameof(Pawn_InteractionsTracker.InteractionsTrackerTick)),
                transpiler: new HarmonyMethod(patchType, nameof(InteractionsTrackerTickTranspiler)));


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
            Logging.Warning("Prefix worked!!");
        }

        public static void TestTranspiler(Pawn pawn)
        {
            Logging.Warning("Transpiler worked!!");
            Logging.Warning($"Pawn {pawn.Name} tried to interact");
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

            // Label jumpToEnd = il.DefineLabel();


            // yield return CodeInstruction.LoadField(typeof(Pawn_InteractionsTracker), "pawn");
            // yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.LoadPawn));
            // yield return new CodeInstruction(OpCodes.Stsfld, AccessTools.Field(typeof(AultoLib_Pawn_InteractionsTracker), "pawn"));
            // yield return CodeInstruction.StoreField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.pawn));
            // yield return CodeInstruction.LoadField(typeof(Pawn_InteractionsTracker), "wantsRandomInteract");
            // yield return CodeInstruction.StoreField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.wantsRandomInteract));
            // yield return CodeInstruction.LoadField(typeof(Pawn_InteractionsTracker), "lastInteraction");
            // yield return CodeInstruction.StoreField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.lastInteraction));
            // yield return CodeInstruction.LoadField(typeof(Pawn_InteractionsTracker), "lastInteractionTime");
            // yield return CodeInstruction.StoreField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.lastInteractionTime));
            // yield return new CodeInstruction(OpCodes.Ldarg_0);
            // yield return CodeInstruction.Call(typeof(Pawn_InteractionsTracker), "get_CurrentSocialMode");
            // yield return CodeInstruction.StoreField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.currentSocialMode));

            // yield return CodeInstruction.Call(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.InteractionsTrackerTick));

            // yield return CodeInstruction.LoadField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.wantsRandomInteract));
            // yield return CodeInstruction.StoreField(typeof(Pawn_InteractionsTracker), "wantsRandomInteract");
            // yield return CodeInstruction.LoadField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.lastInteraction));
            // yield return CodeInstruction.StoreField(typeof(Pawn_InteractionsTracker), "lastInteraction");
            // yield return CodeInstruction.LoadField(typeof(AultoLib_Pawn_InteractionsTracker), nameof(AultoLib_Pawn_InteractionsTracker.lastInteractionTime));
            // yield return CodeInstruction.StoreField(typeof(Pawn_InteractionsTracker), "lastInteractionTime");

            // yield return new CodeInstruction(OpCodes.Ret);
            // yield return new CodeInstruction(OpCodes.Nop) { labels = new List<Label> { jumpToEnd } };


            foreach (CodeInstruction instruction in instructions)
                yield return instruction;
        }
        // /* (64,9)-(64,10) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x0000306C 00           */ IL_0000: nop
        // /* (65,13)-(65,64) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x0000306D 02           */ IL_0001: ldarg.0
        // /* 0x0000306E 7B69000004   */ IL_0002: ldfld     class ['Assembly-CSharp']Verse.Pawn AultoLib._Pawn_InteractionsTracker_edits::pawn
        // /* 0x00003073 8008000004   */ IL_0007: stsfld    class ['Assembly-CSharp']Verse.Pawn AultoLib.AultoLib_Pawn_InteractionsTracker::pawn
        // /* (66,13)-(66,89) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x00003078 02           */ IL_000C: ldarg.0
        // /* 0x00003079 7B6A000004   */ IL_000D: ldfld     bool AultoLib._Pawn_InteractionsTracker_edits::wantsRandomInteract
        // /* 0x0000307E 8009000004   */ IL_0012: stsfld    bool AultoLib.AultoLib_Pawn_InteractionsTracker::wantsRandomInteract
        // /* (67,13)-(67,94) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x00003083 02           */ IL_0017: ldarg.0
        // /* 0x00003084 7B6B000004   */ IL_0018: ldfld int32 AultoLib._Pawn_InteractionsTracker_edits::lastInteractionTime
        // /* 0x00003089 800A000004   */ IL_001D: stsfld int32 AultoLib.AultoLib_Pawn_InteractionsTracker::lastInteractionTime
        // /* (68,13)-(68,86) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x0000308E 02           */ IL_0022: ldarg.0
        // /* 0x0000308F 7B6C000004   */ IL_0023: ldfld     string AultoLib._Pawn_InteractionsTracker_edits::lastInteraction
        // /* 0x00003094 800B000004   */ IL_0028: stsfld    string AultoLib.AultoLib_Pawn_InteractionsTracker::lastInteraction
        // /* (69,13)-(69,90) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x00003099 02           */ IL_002D: ldarg.0
        // /* 0x0000309A 28B3000006   */ IL_002E: call instance valuetype['Assembly-CSharp'] RimWorld.RandomSocialMode AultoLib._Pawn_InteractionsTracker_edits::get_CurrentSocialMode()
        // /* 0x0000309F 8007000004   */ IL_0033: stsfld valuetype ['Assembly-CSharp'] RimWorld.RandomSocialMode AultoLib.AultoLib_Pawn_InteractionsTracker::currentSocialMode
        // /* (70,13)-(70,73) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030A4 2814000006   */ IL_0038: call      void AultoLib.AultoLib_Pawn_InteractionsTracker::InteractionsTrackerTick()
        // /* 0x000030A9 00           */ IL_003D: nop
        // /* (71,13)-(71,94) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030AA 02           */ IL_003E: ldarg.0
        // /* 0x000030AB 7E09000004   */ IL_003F: ldsfld    bool AultoLib.AultoLib_Pawn_InteractionsTracker::wantsRandomInteract
        // /* 0x000030B0 7D6A000004   */ IL_0044: stfld     bool AultoLib._Pawn_InteractionsTracker_edits::wantsRandomInteract
        // /* (72,13)-(72,94) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030B5 02           */ IL_0049: ldarg.0
        // /* 0x000030B6 7E0A000004   */ IL_004A: ldsfld int32 AultoLib.AultoLib_Pawn_InteractionsTracker::lastInteractionTime
        // /* 0x000030BB 7D6B000004   */ IL_004F: stfld int32 AultoLib._Pawn_InteractionsTracker_edits::lastInteractionTime
        // /* (73,13)-(73,86) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030C0 02           */ IL_0054: ldarg.0
        // /* 0x000030C1 7E0B000004   */ IL_0055: ldsfld    string AultoLib.AultoLib_Pawn_InteractionsTracker::lastInteraction
        // /* 0x000030C6 7D6C000004   */ IL_005A: stfld     string AultoLib._Pawn_InteractionsTracker_edits::lastInteraction
        // /* (74,13)-(74,20) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030CB 2B00         */ IL_005F: br.s IL_0061

        // /* (115,9)-(115,10) C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods\AultoLib\Source\_Pawn_InteractionsTracker_edits.cs */
        // /* 0x000030CD 2A           */ IL_0061: ret
    }
}
