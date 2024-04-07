using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using AultoLib.Database;
using RimWorld;
using System;
using System.Text;

namespace AultoLib.Grammar
{
    public static class MacroResolver
    {

        public static bool TryResolve(string rootKeyword, string debugLabel, out string expansion)
        {
            RuleMacro rootMacro = new RuleMacro(rootKeyword);
            if (!TryExpandMacro(rootMacro, out CompoundRule compoundRule))
            {
                Log.Error($"{Globals.LOG_HEADER} Expansion of \"{debugLabel}\" failed");
                expansion = null;
                return false;
            }
            if (doLog) Log.Message($"{Globals.DEBUG_LOG_HEADER} Trying to resolve {debugLabel}");

            //AultoLog.DebugMessage_Advanced($"got CompoundRule {compoundRule.keyword}");
            // get the segments, then call ExpandRecurive(segments)
            HashSet<string> something = new HashSet<string>();
            ExtendedRule rule = compoundRule.RandomPossiblyResolvableEntry(something);
            //AultoLog.DebugMessage_Advanced($"got rule {rule.keyword}");
            RuleSegments segs = rule.GetSegments();
            //AultoLog.DebugMessage_Advanced($"got segments");
            // test segmetns


            logSb = new StringBuilder();
            logSb.AppendLine($"{Globals.DEBUG_LOG_HEADER} Trying to resolve {debugLabel}");
            if (doLog) logSb.AppendLine("==== attempting expansion ====");
            //expansion = ExpandRecursive(segs).ToString();

            StringBuilder sb = new StringBuilder();
            foreach (string part in ExpandRecursive(segs))
            {
                sb.Append(part);
            }
            expansion = sb.ToString();


            // try
            // {
            // }
            // catch (Exception e)
            // {
            //     Log.Error($"{Globals.LOG_HEADER} an exception ocurred: {e}");
            //     expansion = null;
            //     return false;
            // }
            // finally
            // {
            //     if (doLog) Log.Message(logSb.ToString());
            // }

            if (doLog) AultoLibMod.DebugMessage("expansion successful");
            return true;
        }


        // +----------------------+
        // |    Macro Expander    |
        // +----------------------+
        public static IEnumerable<string> ExpandRecursive(RuleSegments segments)
        {
            loopCount = 0;
            HashSet<string> encounterdTags = new HashSet<string>();
            if (ResolverInstance.extraTagSet?.Any() == true) encounterdTags.AddRange(ResolverInstance.extraTagSet); // add extra tags
            foreach (string s in ExpandRecursive(0, segments, encounterdTags))
                yield return s;
            yield break;
        }

        // TODO: make ExpandRecusive modify a StringBuilder instead of returning IEnumerable<string> things

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r">recursion level</param>
        /// <param name="segments"></param>
        /// <param name="encounteredTags"></param>
        /// <returns></returns>
        private static IEnumerable<string> ExpandRecursive(int r, RuleSegments segments, HashSet<string> encounteredTags)
        {
            loopCount++;
            if (doLog) AultoLibMod.DebugMessage($"DOING RECURSION: level {r}, loop {loopCount}, segments: \"{segments}\"");

            if (loopCount > MAX_LOOPS)
            {
                Log.Error($"{Globals.LOG_HEADER} Max loops reached");
                yield break;
            }
            if (r > MAX_RECURSIONS)
            {
                Log.Error($"{Globals.LOG_HEADER} Max recursions reached");
                yield break;
            }

            foreach (RuleSegments.Segment seg in segments.List)
            {
                if (seg.isMacro)
                {
                    if (doLog) AultoLibMod.DebugMessage($"expanding segment \"{seg}\"");
                    yield return seg.text;
                    if (TryExpandMacro(seg.macro, out CompoundRule compoundRule))
                    {
                        // select rule
                        ExtendedRule rule = compoundRule.RandomPossiblyResolvableEntry(encounteredTags);

                        if (rule == null)
                        {
                            if (doLog) AultoLibMod.DebugMessage($"selected rule, but it was null");
                            yield break;
                        }

                        if (doLog) AultoLibMod.DebugMessage($"selected rule {rule.keyword}");

                        // process tags
                        HashSet<string> newTags = new HashSet<string>();
                        newTags.AddRange(encounteredTags);

                        newTags.ProcessTags(rule.tags);
                        ResolverInstance.savedTagSet.ProcessTags(rule.savedTags);

                        // I don't need to save anything from requiredTags because if it got here, it means I already had the right tags.
                        // if (rule.requiredTags != null) newTags.AddRange(rule.requiredTags);

                        // expand rule
                        RuleSegments segs = rule.GetSegments();
                        if (doLog) AultoLibMod.DebugMessage($"got segments: {segs}");

                        // expand segments
                        foreach (string s in ExpandRecursive(r + 1, segs, newTags))
                        {
                            yield return s;
                        }
                    }
                    else Log.Error($"{Globals.LOG_HEADER} macro expansion failed on segment: {seg}");
                }
                else
                {
                    if (doLog) AultoLibMod.DebugMessage($"expanding text \"{seg}\"");
                    yield return seg.text;
                }
            }
            yield break;
        }


        private static void ProcessTags(this HashSet<string> tagsToModify, Dictionary<string,bool> advancedTags)
        {
            if (advancedTags == null) return;
            if (advancedTags?.Any() == false) return;

            IEnumerable<string> include = from item in advancedTags
                                          where item.Value == true
                                          select item.Key;
            IEnumerable<string> exclude = from item in advancedTags
                                          where item.Value == false
                                          select item.Key;

            tagsToModify.ExceptWith(exclude);
            tagsToModify.UnionWith(include);
        }

        /// <summary>
        /// Expands a macro
        /// </summary>
        /// <param name="macro"></param>
        /// <returns></returns>
        public static bool TryExpandMacro(RuleMacro macro, out CompoundRule rule)
        {
            if (macro.isMacro)
            {
                // macro.society can be null
                if (GrammarDatabase.TryGetCompoundRule(macro.society, macro.key, out rule))
                {
                    return true;
                }
                // no macro found
            }
            else
            {
                if (GrammarDatabase.TryGetConstant(macro.key, out string value))
                {
                    rule = new CompoundRule(macro.key);
                    rule.Add(new ExtendedRule_String(macro.key, value));
                    return true;
                }
            }
            rule = null;
            return false;
        }
        
        // // society can be null
        // public static bool TryGetCompoundRule(string society, string key, out CompoundRule rule)
        // {
        //     // try to find the macro
        //     // could be in the local ruleset
        //     // could be in the global ruleset
        //     if (society == null)
        //     {
        //         // look in local ruleset
        //         if (GrammarDatabase.TryGetLocalCompoundRule(key, out rule))
        //             return true;
        //     }
        //     // look in global ruleset
        //     string society2 = society ?? ResolverInstance.ACTIVE_SOCIETY;
        //     if (GrammarDatabase.TryGetGlobalCompoundRule(society2, key, out rule))
        //         return true;

        //     // no macro found
        //     rule = null;
        //     return false;
        // }

        // +---------------------+
        // |    Rule Selector    |
        // +---------------------+

        public static ExtendedRule RandomPossiblyResolvableEntry(this CompoundRule r, HashSet<string> encounteredTags)
        {

            List<ExtendedRule> list = r.rules;
            if (list == null) return null;


            float maxPriority = float.MinValue;
            foreach (ExtendedRule entry in list)
            {
                if (ResolverInstance.ValidateExtendedRule(entry, encounteredTags) && GetSelectionWeight(entry) != 0f)
                {
                    maxPriority = Mathf.Max(maxPriority, entry.Priority);
                }
            }
            IComparer<ExtendedRule> customComparer = ResolverInstance.customizer?.StrictRulePrioritizer();

            if (customComparer != null && list.Count>1)
            {
                // This stuff is from RimWorld
                IComparer<ExtendedRule> comparer = Comparer<ExtendedRule>
                    .Create((ExtendedRule a, ExtendedRule b) => customComparer.Compare(a,b))
                    .ThenBy(
                        Comparer<ExtendedRule>
                        .Create((ExtendedRule a, ExtendedRule b) => GetSelectionWeight(a).CompareTo(GetSelectionWeight(b)) )
                        .Descending<ExtendedRule>()
                    );
                List<ExtendedRule> tmpSortedList = new List<ExtendedRule>();
                foreach (ExtendedRule entry in list)
                {
                    if (ResolverInstance.ValidateExtendedRule(entry, encounteredTags) && entry.Priority == maxPriority)
                    {
                        tmpSortedList.Add(entry);
                    }
                }
                tmpSortedList.Shuffle<ExtendedRule>();
                tmpSortedList.TryMinBy((ExtendedRule x) => x, comparer, out ExtendedRule result);
                return result;
            }

            // the regular return method thing
            return list.RandomElementByWeightWithFallback(delegate (ExtendedRule rule)
            {
                if (!(rule.Priority == maxPriority && ResolverInstance.ValidateExtendedRule(rule, encounteredTags)))
                    return 0f;
                else
                    return GetSelectionWeight(rule);
            }, null);
        }


        private static float GetSelectionWeight(ExtendedRule rule)
        {
            return rule.Weight * 100000f / (float)((rule.uses + 1) * 1000);
        }

        // +---------------+
        // |    Loaders?   |
        // +---------------+

        // +-----------------+
        // |    Variables    |
        // +-----------------+

        private static int loopCount;
        public const int MAX_RECURSIONS = 64;
        public const int MAX_LOOPS = 1024;

        public static bool doLog = false;
        public static StringBuilder logSb;

    }
}
