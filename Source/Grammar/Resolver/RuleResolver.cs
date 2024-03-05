using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Verse;
using Verse.Grammar;
using AultoLib.Grammar.Rule;
using AultoLib.Database;

namespace AultoLib.Grammar.Resolver
{
    /// <summary>
    /// This is for an experiment, to understand this class. then I will rewrite it later.
    /// make this very similar to rimworlds one, then rewrite it
    /// </summary>
    public static class RuleResolver
    {
        public static string Resolve(
            string rootKeyword,
            NewGrammarRequest request,
            string debugLabel = null,
            bool forceLog = false,
            string untranslatedRootKeyword = null,
            List<string> extraTags = null,
            List<string> outTags = null,
            bool capitalizeFirstSentence = true)
        {
            // Doesn't support languages other than english, so

            ResolverData.debugLabel = debugLabel;
            ResolverData.forceLog = forceLog;
            ResolverData.extraTags = extraTags;
            ResolverData.outTags = outTags;
            ResolverData.capitalizeFirstSentence = capitalizeFirstSentence;

            return RuleResolver.Resolve(rootKeyword, request);
        }

        public static string Resolve(string rootKeyword, NewGrammarRequest request)
        {

            RuleResolver.REQUEST = request;
            RuleResolver.SETTINGS = settings;
            bool success;
            string result = RuleResolver.ResolveUnsafe(rootKeyword, out success);
            RuleResolver.REQUEST = null;
            RuleResolver.SETTINGS = null;
            return result;

        }
        
        public static string ResolveUnsafe(string rootKeyword, out bool success)
        {
            bool doLog = SETTINGS.forceLog || DebugViewSettings.logGrammarResolution;
            RuleResolver.rules.Clear();
            RuleResolver.rulePool.Clear();
            if (doLog)
            {
                RuleResolver.logSbTrace = new StringBuilder();
                RuleResolver.logSbMid = new StringBuilder();
                RuleResolver.logSbRules = new StringBuilder();
            }

            if (REQUEST.RulesAllowNull != null)
            {
                if (doLog) RuleResolver.logSbRules.AppendLine("CUSTOM RULES");
                foreach (ExtendedRule rule in REQUEST.RulesAllowNull)
                {
                    RuleResolver.AddRule(rule);
                    if (doLog) RuleResolver.logSbRules.AppendLine($"■{rule}");
                }
                if (doLog) RuleResolver.logSbRules.AppendLine();
            }

            if (REQUEST.IncludesAllowNull != null)
            {
                HashSet<ExtendedRulePackDef> hashSet = new HashSet<ExtendedRulePackDef>();
                List<ExtendedRulePackDef> defList = new List<ExtendedRulePackDef>(REQUEST.IncludesAllowNull);
                if (doLog) RuleResolver.logSbMid.AppendLine("INCLUDES");

                while (defList.Count > 0)
                {
                    ExtendedRulePackDef extendedRulePackDef = defList[defList.Count - 1];
                    defList.RemoveLast();
                    if (!hashSet.Contains(extendedRulePackDef))
                    {
                        if (doLog) RuleResolver.logSbMid.AppendLine($"{{{extendedRulePackDef.defName}}}");
                        hashSet.Add(extendedRulePackDef);
                        if (extendedRulePackDef.RulesImmediate != null)
                        {
                            foreach (ExtendedRule rule in extendedRulePackDef.RulesImmediate)
                            {
                                RuleResolver.AddRule(rule);
                            }
                        }
                        if (!extendedRulePackDef.include.NullOrEmpty<ExtendedRulePackDef>())
                        {
                            defList.AddRange(extendedRulePackDef.include);
                        }
                    }
                }
            }


            if (REQUEST.IncludesBareAllowNull != null)
            {
                if (doLog)
                {
                    RuleResolver.logSbMid.AppendLine();
                    RuleResolver.logSbMid.AppendLine("BARE INCLUDES");
                }
                foreach (ExtendedRulePack rulePack in REQUEST.IncludesBareAllowNull)
                {
                    foreach (ExtendedRule rule in rulePack.Rules)
                    {
                        RuleResolver.AddRule(rule);
                        if (doLog) RuleResolver.logSbMid.AppendLine($"  {rule}");
                    }
                }
            }


            if (doLog && !SETTINGS.extraTags.NullOrEmpty())
            {
                RuleResolver.logSbMid.AppendLine();
                RuleResolver.logSbMid.AppendLine("EXTRA TAGS");
                foreach (string tag in SETTINGS.extraTags)
                    RuleResolver.logSbMid.AppendLine($"  {tag}");
            }

            // supposed to do a thing here where global rules are added. 

            RuleResolver.loopCount = 0;

            Dictionary<string, string> constantsAllowNull = REQUEST.ConstantsAllowNull;
            if (doLog && constantsAllowNull != null)
            {
                RuleResolver.logSbMid.AppendLine("CONSTANTS");
                foreach(KeyValuePair<string,string> keyValuePair in constantsAllowNull)
                {
                    RuleResolver.logSbMid.AppendLine(keyValuePair.Key.PadRight(38) + " " + keyValuePair.Value);
                }
            }
            if (doLog) RuleResolver.logSbTrace.Append("GRAMMAR RESOLUTION TRACE");

            string text = "err";
            bool flag = false;
            List<string> list = new List<string>();
            if (RuleResolver.TryResolveRecursive(new RuleResolver.ExtendedRuleEntry(new ExtendedRule_String("", $"[{rootKeyword}]")), 0, constantsAllowNull, out text, request, settings ))
            {
                if (SETTINGS.outTags != null)
                {
                    SETTINGS.outTags.Clear();
                    SETTINGS.outTags.AddRange(list);
                }
            }
            else
            {
                flag = true;
                if (REQUEST.Rules.NullOrEmpty<ExtendedRule>())
                    text = "ERR";
                else
                    text = "ERR: " + REQUEST.Rules[0].Generate();

                if (doLog)
                {
                    RuleResolver.logSbTrace.Insert(0, "Grammar unresolvable. Root '" + rootKeyword + "'\n\n");
                }
                else
                {
                    bool throwAway;
                    RuleResolver.ResolveUnsafe(rootKeyword, out throwAway);
                }

            }

            text = RuleResolver.ResolveAllFunctions(text, constantsAllowNull);
            text = GenText.CapitalizeSentences(Find.ActiveLanguageWorker.PostProcessed(text), SETTINGS.capitalizeFirstSentence);
            text = RuleResolver.Spaces.Replace(text, (Match match) => match.Groups[1].Value);
            text = text.Trim();

            if (doLog)
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(RuleResolver.logSbTrace.ToString().TrimEndNewlines());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(RuleResolver.logSbMid.ToString().TrimEndNewlines());
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(RuleResolver.logSbRules.ToString().TrimEndNewlines());
                string message = stringBuilder.ToString().Trim();
                if (flag)
                {
                    if (DebugViewSettings.logGrammarResolution)
                        Log.Error(message + "\n");
                    else
                        Log.ErrorOnce(message + "\n", message.GetHashCode());
                }
                else
                {
                    Log.Message(message + "\n");
                }
                RuleResolver.logSbTrace = null;
                RuleResolver.logSbMid = null;
                RuleResolver.logSbRules = null;
            }

            success = !flag;
            return text;
        }


        public static string ResolveAllFunctions(string input, Dictionary<string, string> constants)
        {
            if (input.Contains("%%"))
                return input;

            string text = input;
            MatchCollection matches = RuleResolver.FunctionCall.Matches(text);
            foreach (Match match in matches)
            {
                string value = match.Groups[1].Value;
                string args = match.Groups[2].Value.Trim();
                if (value == "lookup" || value == "replace" )
                {
                    string value2 = GrammarResolverSimple.ResolveFunction(value, args, match.Value);
                    text = text.Remove(match.Index, match.Length);
                    text = text.Insert(match.Index, value2);
                }
                else
                {
                    Log.Warning($"Unknown grammar function name: {value}. Supported functions: lookup, replace.");
                }
            }
            MatchCollection matches2 = RuleResolver.GenderCall.Matches(text);
            for (int i = matches2.Count - 1; i >= 0; i--)
            {
                Match match2 = matches2[i];
                string value2 = match2.Groups[1].Value;
                string args2 = match2.Groups[2].Value.Trim();
                string text2;
                if (constants.TryGetValue($"{value2}_gender", out text2))
                {
                    Gender gender;
                    if (Enum.TryParse<Gender>(text2, out gender))
                    {
                        string value3 = GrammarResolverSimple.ResolveGenderSymbol(gender, false, args2, match2.Value);
                        text = text.Remove(match2.Index, match2.Length);
                        text = text.Insert(match2.Index, value3);
                    }
                    else
                    {
                        Log.Warning("Unknown gender: " + text2 + ".");
                    }
                }
                else
                {
                    Log.Warning("Cannot find rules for pawn symbol " + value2 + ".");
                }
            }

            return text;
        }

        // I won't need this because I have the grammar database
        private static void AddRule(ExtendedRule rule)
        {
            List<RuleResolver.ExtendedRuleEntry> list = null;
            if (!RuleResolver.rules.TryGetValue(rule.keyword, out list))
            {
                list = RuleResolver.rulePool.Get();
                list.Clear();
                RuleResolver.rules[rule.keyword] = list;
            }
            list.Add(new RuleResolver.ExtendedRuleEntry(rule)); // <-- idk what that does
        }


        private static bool TryResolveRecursive(RuleResolver.ExtendedRuleEntry entry, int depth, out string output)
        {
            bool doLog = SETTINGS.forceLog || DebugViewSettings.logGrammarResolution;
            Dictionary<string, string> constants = REQUEST.Constants;

            string text = "";
            for (int i = 0; i < depth; i++) { text += "  "; } // padding?
            if (doLog && depth>0)
            {
                RuleResolver.logSbTrace.AppendLine();
                RuleResolver.logSbTrace.Append(depth.ToStringCached().PadRight(3));
                RuleResolver.logSbTrace.Append(text + entry);
            }
            text += "     ";
            RuleResolver.loopCount++;
            if (RuleResolver.loopCount > RuleResolver.LoopsLimit)
            {
                Log.Error($"{Globals.LOG_HEADER} Hit loops limit resolving grammar.");
                output = "HIT_LOOPS_LIMIT";
                if (doLog)
                    RuleResolver.logSbTrace.Append("\n" + text + "UNRESOLVABLE: Hit loops limit");
                return false;
            }
            if (depth > DepthLimit)
            {
                Log.Error($"{Globals.LOG_HEADER} Grammar recurred too deep while resolving keyword (>" + 50 + " deep)");
                output = "DEPTH_LIMIT_REACHED";
                if (doLog)
                    RuleResolver.logSbTrace.Append("\n" + text + "UNRESOLVABLE: Depth limit reached");
                return false;
            }

            string text2 = entry.rule.Generate();
            bool flag = false;
            int num = -1;
            for (int j = 0; j < text2.Length; j++ )
            {
                char c = text2[j];
                if (c == '[')
                    num = j;
                if (c == ']')
                {
                    if (num == -1)
                    {
                        Log.Error($"{Globals.LOG_HEADER} Could not resolve rule because of mismatched brackets: {text2}");
                        output = "MISMATCHED_BRACKETS";
                        if (doLog)
                            RuleResolver.logSbTrace.Append($"\n" + text + "UNRESOLVABLE: Mismatched brackets");
                        flag = true;
                    }
                    else
                    {
                        string text3 = text2.Substring(num+1, j-num-1 );
                        RuleResolver.ExtendedRuleEntry ruleEntry;
                        List<string> list;
                        string str = "UNNASSIGNED";
                        for (;;)
                        {
                            ruleEntry = RuleResolver.RandomPossiblyResolvableEntry(text3);
                            if (ruleEntry == null)
                                break;
                            ruleEntry.uses++;
                            list = SETTINGS.resolvedTags.ToList();
                            if (RuleResolver.TryResolveRecursive(ruleEntry, depth + 1, out str))
                                goto SuccessfulyResolved;
                            ruleEntry.MarkKnownUnresolvable();
                        }
                        entry.MarkKnownUnresolvable();
                        output = "CANNOT_RESOLVE_SUBSYMBOL:" + text3;
                        if (doLog) RuleResolver.logSbTrace.Append($"\n{text}{text3}->UNRESOLVABLE");
                        flag = true;
                        goto EndLoop;
                    SuccessfulyResolved:
                        text2 = text2.Substring(0, num) + str + text2.Substring(j + 1);
                        j = num;
                        if (!ruleEntry.rule.tag.NullOrEmpty() && !SETTINGS.resolvedTags.Contains(ruleEntry.rule.tag))
                        {
                            SETTINGS.resolvedTags.Add(ruleEntry.rule.tag);
                        }
                        if (REQUEST.customizer != null)
                        {
                            REQUEST.customizer.Notify_RuleUsed(ruleEntry.rule);
                        }


                    }
                }
            }




            return false;
        }
        // +------------------------------+
        // |     Important Variables      |
        // +------------------------------+

        private static NewGrammarRequest REQUEST;
        private static ResolverSettings SETTINGS;

        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        private static SimpleLinearPool<List<RuleResolver.ExtendedRuleEntry>> rulePool = new SimpleLinearPool<List<RuleResolver.ExtendedRuleEntry>>();
        private static Dictionary<string, List<RuleResolver.ExtendedRuleEntry>> rules = new Dictionary<string, List<RuleResolver.ExtendedRuleEntry>>();
        private static int loopCount;
        private static StringBuilder logSbTrace;
        private static StringBuilder logSbMid;
        private static StringBuilder logSbRules;
        private const int DepthLimit = 50;
        private const int LoopsLimit = 1000;
        public const char SymbolStartChar = '['; // idk why this is public
        public const char SymbolEndChar = ']';
        private static readonly char[] SpecialChars = new char[] { '[', ']', '{', '}' };
        private static Regex Spaces = new Regex(" +([,.])");
        private static Regex FunctionCall = new Regex("{\\s*(\\w+)\\s*\\:\\s*([^}]* ?)\\s*}");
        private static Regex GenderCall = new Regex("{\\s*(\\w+)_gender\\s*\\?\\s*([^}]* ?)\\s*}");
        private static List<RuleResolver.ExtendedRuleEntry> tmpSortedRuleList = new List<RuleResolver.ExtendedRuleEntry>();

        private class ExtendedRuleEntry
        {
            public float SelectionWeight
            {
                get
                {
                    return this.rule.Weight * 100000f / (float)((this.uses + 1) * 1000);
                }
            }

            public float Priority
            {
                get { return this.rule.Priority; }
            }

            public ExtendedRuleEntry(ExtendedRule rule)
            {
                this.rule = rule;
                this.knownUnresolvable = false;
            }

            public void MarkKnownUnresolvable()
            { this.knownUnresolvable = true; }

            public bool ValidateConstantConstraints(Dictionary<string, string> constraints)
            {
                if (!this.constantConstraintsChecked)
                {
                    this.constantConstraintsValid = true;
                    if (this.rule.constraintList != null)
                        this.constantConstraintsValid = this.rule.ValidateConstraints(constraints);
                    this.constantConstraintsChecked = true;
                }
                return this.constantConstraintsValid;
            }

            public bool ValidateRequiredTag(List<string> extraTags, List<string> resolvedTags)
            { return this.rule.requiredTag.NullOrEmpty() || (extraTags != null && extraTags.Contains(this.rule.requiredTag)) || resolvedTags.Contains(this.rule.requiredTag); }

            public bool ValidateTimesUsed()
            { return this.rule.usesLimit == null || this.uses < this.rule.usesLimit.Value; }

            public override string ToString()
            { return this.rule.ToString(); }

            public ExtendedRule rule;
            public bool knownUnresolvable;
            public bool constantConstraintsChecked;
            public bool constantConstraintsValid;
            public int uses;
        }

    }
}
