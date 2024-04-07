using System;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Verse;
using JetBrains.Annotations;
using UnityEngine;

namespace AultoLib
{
    public static class LogColor
    {
        // public static readonly Color Header = GenColor.FromHex("ffa500");
        // public static readonly Color ClassName = GenColor.FromHex("C09A1B");
        // public static readonly Color Def = GenColor.FromHex("AA7F00");
        // public static readonly Color DefName = GenColor.FromHex("A69564");
        public static readonly string Header = "#ffa500";
        public static readonly string ClassName = "#C09A1B";
        public static readonly string Def = "#AA7F00";
        public static readonly string DefName = "#A69564";
    }

    public static class Logging
    {
        private static readonly Dictionary<Type, LogData> classLogging = new Dictionary<Type, LogData>();


        // public static bool DoLog(Type C, string tag = null)
        // {
        //     if (!classLogging.TryGetValue(C, out var log))
        //     {
        //         Logging.Error($"Class \"{nameof(C)}\" isn't set up for logging");
        //         return false;
        //     }
        //     return log.ShouldDoLog(tag);
        // }

        // public static bool DoLog(string tag = null)
        // {
        //     MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
        //     if (!classLogging.TryGetValue(caller.ReflectedType, out var log))
        //     {
        //         Logging.Error($"Class \"{nameof(caller.ReflectedType)}\" isn't set up for logging");
        //         return false;
        //     }
        //     return log.ShouldDoLog(tag);
        // }

        public static LogData SetupLogging(Type C, bool enabled, params string[] possibleTags)
        {
            // create LogData
            LogData data = new LogData(C, enabled, possibleTags);

            // add that to the database
            classLogging[C] = data;
            return data;
        }

        public static LogData GetLoggingData(Type C)
        {
            if (classLogging.TryGetValue(C, out var log))
            {
                return log;
            }
            else Logging.SimpleError($"Class \"{C.FullName}\" isn't set up for logging");
            return null;
        }

        // public static void EnableLogging(Type C, bool enabled)
        // {
        //     if (classLogging.TryGetValue(C, out var log))
        //     {
        //         log.enabled = enabled;
        //     }
        //     else Logging.Error($"Class \"{nameof(C)}\" isn't set up for logging");
        // }
        // public static void SetLoggingTags(Type C, bool toEnable, params string[] tags)
        // {
        //     if (classLogging.TryGetValue(C, out var log))
        //     {
        //         log.SetTags(toEnable, tags);
        //     }
        //     else Logging.Error($"Class \"{nameof(C)}\" isn't set up for logging");
        // }


        public class LogData
        {
            // is this neccesary?
            // public LogData(Type ClassType, bool enabled)
            // {
            //     this.ClassType = ClassType;
            //     this.enabled = enabled;
            //     this.possibleTags = null;
            //     this.enabledTags = null;
            // }
            public LogData(Type ClassType, bool enabled, params string[] possibleTags)
            {
                this.ClassType = ClassType;
                this.enabled = enabled;
                this.possibleTags = new HashSet<string>(possibleTags);
                this.enabledTags = null;
            }


            public bool ShouldDoLog(string tag = null)
            {
                if (!enabled) return false;
                if (tag == null) return true;
                if (!possibleTags.Contains(tag))
                {
                    Logging.Warning($"the tag {tag} wasn't one of \"{ClassType.FullName}\"'s possible tags");
                    return false;
                }
                if (enabledTags.Contains(tag)) return true;
                return false;
            }

            public LogData PossibleTags(params string[] possibleTags)
            {
                foreach (string tag in possibleTags)
                {
                    this.possibleTags.Add(tag);
                }
                return this;
            }

            public LogData SetTags(bool toEnable, params string[] tags)
            {
                if (enabledTags == null) enabledTags = new HashSet<string>();
                foreach (string tag in tags)
                {
                    if (!possibleTags.Contains(tag))
                    {
                        Logging.Warning($"Can't find tag {tag} in {ClassType.FullName}");
                    }
                    else
                    {
                        if (toEnable) enabledTags.Add(tag);
                        else enabledTags.Remove(tag);
                    }
                }
                return this;
            }

            // public LogData EnableTags(params string[] tags)
            // {
            //     return this.SetTags(true, tags);
            // }

            public Type ClassType;
            public bool enabled;
            public HashSet<string> possibleTags;
            private HashSet<string> enabledTags;
        }


        // +---------------+
        // |    Logging    |
        // +---------------+

        public static bool DoLog(string tag = null)
        {
            MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            if (!classLogging.TryGetValue(caller.ReflectedType, out var log))
            {
                Logging.SimpleErrorOnce($"Class \"{caller.ReflectedType.FullName}\" isn't set up for logging", "ClassNotSetUpForLogging");
                return false;
            }
            return log.ShouldDoLog(tag);
        }


        // public static void Message_Basic(string text) => Log.Message($"{AultoLibMod.LOG_HEADER} {text}");
        // public static void Warning_Basic(string text) => Log.Warning($"{AultoLibMod.LOG_HEADER} {text}");
        // public static void Error_Basic(string text) => Log.Error($"{AultoLibMod.LOG_HEADER} {text}");

        // public static void DebugMessage_Basic(string text) => Log.Message($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");
        // public static void DebugWarning_Basic(string text) => Log.Warning($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");
        // public static void DebugError_Basic(string text) => Log.Error($"{AultoLibMod.DEBUG_LOG_HEADER} {text}");
        public static string SimplePrefix()
        {
            MethodBase caller = new StackTrace().GetFrame(2).GetMethod();
            // Assembly callerNamespace = caller.DeclaringType.Assembly;
            string callerNamespace = caller.DeclaringType.Assembly.GetName().Name;
            // return $"<color=orange>[AultoLib] {className}</color>";
            return $"<color=orange>[{callerNamespace}]</color>"; // now it can be called by other namespaces
        }

        public static string AdvancedPrefix()
        {
            MethodBase caller = new StackTrace().GetFrame(2).GetMethod();
            // Assembly callerNamespace = caller.DeclaringType.Assembly;
            string callerNamespace = caller.DeclaringType.Assembly.GetName().Name;
            string className = caller.ReflectedType.Name;
            // return $"<color=orange>[AultoLib] {className}</color>";
            return $"<color=orange>[{callerNamespace}]</color> {className.ColorText(LogColor.ClassName)}"; // now it can be called by other namespaces
        }

        public static void SimpleMessage(string text) => Log.Message($"{SimplePrefix()}  {text}");
        public static void SimpleWarning(string text) => Log.Warning($"{SimplePrefix()}  {text}");
        public static void SimpleError(string text) => Log.Error($"{SimplePrefix()}  {text}");

        public static void Message(string text) => Log.Message($"{AdvancedPrefix()}  {text}");
        public static void Warning(string text) => Log.Warning($"{AdvancedPrefix()}  {text}");
        public static void Error(string text) => Log.Error($"{AdvancedPrefix()}  {text}");
        public static void DebugMessage(string text) => Log.Message($"{AdvancedPrefix()} {DEBUG}  {text}");
        public static void DebugWarning(string text) => Log.Warning($"{AdvancedPrefix()} {DEBUG}  {text}");
        public static void DebugError(string text) => Log.Error($"{AdvancedPrefix()} {DEBUG}   {text}");


        public static string ColoredDefInformation(this Def someDef)
        {
            string defClass = someDef.GetType().Name;
            string defName = someDef.defName;
            return defClass.ColorText(LogColor.Def) + " " + defName.ColorText(LogColor.DefName);
        }

        public static void SimpleErrorOnce(string text, string id)
        {
            if (logIDs.Contains(id)) return;
            logIDs.Add(id);
            // MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            // string callerNamespace = caller.DeclaringType.Assembly.GetName().Name;
            // string className = caller.ReflectedType.Name;
            Log.Error($"{SimplePrefix()}  {text}");
        }
        public static void ErrorOnce(string text, string id)
        {
            if (logIDs.Contains(id)) return;
            logIDs.Add(id);
            // MethodBase caller = new StackTrace().GetFrame(1).GetMethod();
            // string callerNamespace = caller.DeclaringType.Assembly.GetName().Name;
            // string className = caller.ReflectedType.Name;
            Log.Error($"{AdvancedPrefix()}  {text}");
        }

        public static readonly string header_color = "orange";
        public const string DEBUG = "<color=aqua>Debug</color>";
        public static readonly string LOG_HEADER = $"<color={header_color}>[AultoLib]</color>";
        public static readonly string DEBUG_LOG_HEADER = $"<color={header_color}>[AultoLib] <color=aqua>Debug</color></color>";
        public static readonly string RESOLVER_HEADER = $"<color={header_color}>[AultoLib:Resolver]</color>";

        private static readonly HashSet<string> logIDs = new HashSet<string>();

        // +-----------------------+
        // |    Color Utilities    |
        // +-----------------------+

        /// <summary>
        /// text must be some object that implements ToString<br/>
        /// some colors and their names here: https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/StyledText.html#ColorNames
        /// </summary>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ColorText(this object text, string color)
        {
            return $"<color=\"{color}\">{text}</color>";
        }
        
    }
}
