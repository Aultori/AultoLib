﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

using static AultoLib.AultoLog;

namespace AultoLib
{
    public class AultoLibMod : Mod
    {


        public AultoLibMod(ModContentPack content) : base(content)
        {
            AultoLog.Message("Hello from AultoLib!");
            AultoLibMod.LoggingConfig();
        }

        public static void LoggingConfig()
        {
            SetupLogging(typeof(AultoLib.Database.ResolverInstance), false);
            SetupLogging(typeof(AultoLib.Database.TextFile_Loader), true);
            SetupLogging(typeof(AultoLib.Grammar.Ruleset), false);
            SetupLogging(typeof(AultoLib.Grammar.MacroResolver), false)
                .PossibleTags("debug1", "showSteps") 
                .SetTags(true, "debug1");
            SetupLogging(typeof(AultoLib.AultoLib_Pawn_InteractionsTracker), false);
            SetupLogging(typeof(AultoLib.PlayLogEntry_InteractionInstance), false);
            SetupLogging(typeof(AultoLib.CommunicationUtility), true);
            SetupLogging(typeof(AultoLib.SocietyDef), true);

            SetupLogging(typeof(AultoLib.HarmonyPatches), true);
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
