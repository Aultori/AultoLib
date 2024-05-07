using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

using static AultoLib.AultoLog;
using UnityEngine;

namespace AultoLib
{
    public class AultoLibMod : Mod
    {
        public const string Id      = "AultoLib";
        public const string Name    = "AultoLib";
        public const string Version = "0.0";

        public static AultoLibSettings settings;
        public static Mod Instance;

        public AultoLibMod(ModContentPack content) : base(content)
        {
            Instance = this;
            settings = GetSettings<AultoLibSettings>();

            AultoLog.Message("Hello from AultoLib!");
            AultoLibMod.LoggingConfig();
        }

        public static void LoggingConfig()
        {
            _=SetupLogging(typeof(AultoLib.Database.ResolverInstance), false);
            _=SetupLogging(typeof(AultoLib.Database.TextFile_Loader), true);
            _=SetupLogging(typeof(AultoLib.Grammar.Ruleset), false);
            _=SetupLogging(typeof(AultoLib.Grammar.MacroResolver), false)
                .PossibleTags("debug1", "showSteps") 
                .SetTags(true, "debug1");
            _=SetupLogging(typeof(AultoLib.AultoLib_Pawn_InteractionsTracker), false);
            _=SetupLogging(typeof(AultoLib.PlayLogEntry_InteractionInstance), false);
            _=SetupLogging(typeof(AultoLib.CommunicationUtility), true);
            _=SetupLogging(typeof(AultoLib.SocietyDef), true);

            _=SetupLogging(typeof(AultoLib.HarmonyPatches), true);
        }

        /// <summary>
        /// The (optional) GUI part to set your settings.
        /// </summary>
        /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Interactions Enabled", ref settings.interactions, tooltip: "tooltiptooltiptooltiptooltiptooltiptooltip");
            listingStandard.CheckboxLabeled("Interaction Header", ref settings.doInteractionHeader, tooltip: "Add extra information about the interaction to the begging of a pawn's interaction text.");
            //listingStandard.Label("exampleFloatExplanation");
            //settings.exampleFloat = listingStandard.Slider(settings.exampleFloat, 100f, 300f);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        /// <summary>
        /// Override SettingsCategory to show up in the list of settings.
        /// Using .Translate() is optional, but does allow for localisation.
        /// </summary>
        /// <returns>The (translated) mod name.</returns>
        public override string SettingsCategory()
        {
            return "AultoLib";
        }

        // +---------------+
        // |    Logging    |
        // +---------------+

        // public static void Message_Basic(string text) => Log.Message($"{AultoLibMod.LOG_HEADER} {text}");
        // public static void Warning_Basic(string text) => Log.Warning($"{AultoLibMod.LOG_HEADER} {text}");
        // public static void Error_Basic(string text) => Log.Error($"{AultoLibMod.LOG_HEADER} {text}");

        // public static void DebugMessage_Basic(string text) => Log.Message($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");
        // public static void DebugWarning_Basic(string text) => Log.Warning($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");
        // public static void DebugError_Basic(string text) => Log.Error($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");

        // private static string AdvancedPrefix()
        // {
        //     MethodBase caller = new StackTrace().GetFrame(2).GetMethod();
        //     string className = caller.ReflectedType.Name;
        //     return $"<color=orange>[AultoLib] {className}</color>";
        // }

        // public static void Message(string text) => Log.Message($"{AdvancedPrefix()}  {text}");
        // public static void Warning(string text) => Log.Warning($"{AdvancedPrefix()}  {text}");
        // public static void Error(string text) => Log.Error($"{AdvancedPrefix()}  {text}");
        // public static void DebugMessage(string text) => Log.Message($"{AdvancedPrefix()} {DEBUG}  {text}");
        // public static void DebugWarning(string text) => Log.Warning($"{AdvancedPrefix()} {DEBUG}  {text}");
        // public static void DebugError(string text) => Log.Error($"{AdvancedPrefix()} {DEBUG}   {text}");

        // public static string DefTypeAndName(Def someDef)
        // {
        //     string defClass = someDef.GetType().Name;
        //     string defName = someDef.defName;

        //     return $"<color=yellow>{defClass}</color> {defName}";
        // }

        // public static void DidToDef(string actionDescription, Def someDef)
        // {
        //     string defClass = someDef.GetType().Name;
        //     string defName = someDef.defName;

        //     Log.Message($"<color={header_color}>[AultoLib]</color> {actionDescription} <color=yellow>{defClass}:</color> {defName}");
        // }


        // public static void ErrorOnce(string text, string id)
        // {
        //     if (logIDs.Contains(id)) return;
        //     logIDs.Add(id);
        //     MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
        //     string className = caller.ReflectedType.Name;
        //     Log.Error($"<color={header_color}>[AultoLib] {className}</color> {text}");
        // }
        // public static void DebugErrorOnce(string text, string id)
        // {
        //     if (logIDs.Contains(id)) return;
        //     logIDs.Add(id);
        //     MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
        //     string className = caller.ReflectedType.Name;
        //     Log.Error($"<color={header_color}>[AultoLib] {className}</color> <color=aqua>Debug</color> {text}");
        // }

        // public static readonly string header_color = "orange";
        // public const string DEBUG = "<color=aqua>Debug</color>";
        // public static readonly string LOG_HEADER = $"<color={header_color}>[AultoLib]</color>";
        // public static readonly string DEBUG_LOG_HEADER = $"<color={header_color}>[AultoLib] <color=aqua>Debug</color></color>";
        // public static readonly string RESOLVER_HEADER = $"<color={header_color}>[AultoLib:Resolver]</color>";

        // private static readonly HashSet<string> logIDs = new HashSet<string>();

    }
}
