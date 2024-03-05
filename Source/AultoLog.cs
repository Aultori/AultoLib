using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// This adds a header to log messages before printing them to RimWorld
    /// 
    /// For use instead of RimWord's Log class.
    /// 
    /// </summary>
    public static class AultoLog
    {
        public static void Message(string text) => Log.Message($"{AultoLog.LOG_HEADER} {text}");
        public static void Warning(string text) => Log.Warning($"{AultoLog.LOG_HEADER} {text}");
        public static void Error(string text) => Log.Error($"{AultoLog.LOG_HEADER} {text}");

        public static void Message_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Message($"<color={header_color}>[AultoLib] {className}</color> {text}");
        }
        public static void Warning_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Warning($"<color={header_color}>[AultoLib] {className}</color> {text}");
        }
        public static void Error_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Error($"<color={header_color}>[AultoLib] {className}</color> {text}");
        }

        public static void DebugMessage(string text) => Log.Message($"{AultoLog.DEBUG_LOG_HEADER} {text}");
        public static void DebugWarning(string text) => Log.Warning($"{AultoLog.DEBUG_LOG_HEADER} {text}");
        public static void DebugError(string text) => Log.Error($"{AultoLog.DEBUG_LOG_HEADER} {text}");
        public static void DebugMessage_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Message($"<color={header_color}>[AultoLib] {className}</color> <color=aqua>Debug</color> {text}");
        }
        public static void DebugWarning_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Warning($"<color={header_color}>[AultoLib] {className}</color> <color=aqua>Debug</color> {text}");
        }
        public static void DebugError_Advanced(string text)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            string className = caller.ReflectedType.Name;
            Log.Error($"<color={header_color}>[AultoLib] {className}</color> <color=aqua>Debug</color> {text}");
        }

        public static readonly string header_color = "orange";
        public const string DEBUG = "<color=aqua>Debug</color>";
        public static readonly string LOG_HEADER = $"<color={header_color}>[AultoLib]</color>";
        public static readonly string DEBUG_LOG_HEADER = $"<color={header_color}>[AultoLib] <color=aqua>Debug</color></color>";
        public static readonly string RESOLVER_HEADER = $"<color={header_color}>[AultoLib:Resolver]</color>";
    }
}
