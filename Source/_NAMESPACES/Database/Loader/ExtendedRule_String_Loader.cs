using System.Collections.Generic;
using System.Text.RegularExpressions;
using Verse;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    public class ExtendedRule_String_Loader
    {
        public static Ruleset ToRuleset(List<string> rulesStrings)
        {
            Ruleset pack = new Ruleset();

            foreach (string rawString in rulesStrings)
            {
                if (TryBuildRule(rawString, out ExtendedRule_String rule))
                    pack.Add(rule);
            }

            return pack;
        }

        public static bool TryBuildRule(string rawString, out ExtendedRule_String stringRule)
        {

            // Log.Message($"{Globals.DEBUG_LOG_HEADER} building rule from string: \"{rawString}\"");
            ExtendedRule rule = new ExtendedRule_String();
            stringRule = new ExtendedRule_String();

            if (!ExtendedRule_Loader.TryBuildRule(rawString, rule, out string output))
            {
                stringRule = null;
                return false;
            }
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} building {rule.keyword} = \"{output}\"");
            if (!TryBuildSegments(output, out RuleSegments segments))
            {
                stringRule = null;
                return false;
            }

            stringRule.BecomeCopyOf(rule);
            stringRule.segmentList = segments;

            return true;
        }

        public static bool TryBuildSegments(string input, out RuleSegments segments)
        {
            // most segments will look like this: " text here [PREFIX_value"
            // the last one looks like this: " text here."

            segments = new RuleSegments();


            foreach (string segment in input.Split(']'))
            {
                string[] tmp = segment.Split('[');
                if (tmp.Length == 2)
                {
                    string text = tmp[0];
                    RuleMacro macro;
                    if (!TryBuildMacro(tmp[1], out macro)) return false;
                    segments.Add(text, macro);
                }
                else if (tmp.Length == 1)
                {
                    segments.Add(tmp[0]);
                }
                else
                {
                    Log.Error($"{Globals.LOG_HEADER} Something bad happened while generating RuleSegments");
                    return false;
                }
            }

            return true;
        }

        private static bool TryBuildMacro(string macroText, out RuleMacro macro)
        {
            macro = new RuleMacro(macroText);

            return macro.MadeSuccesfully;

            // Regex testforLocalConstant = new Regex("^(?>[A-Z]+)_(?>[A-Za-z]+)$");
            // Regex testforCulture = new Regex("^(?<society>(?>[A-Z]+)(?=\\|))?\\|?(?<key>(?>[A-Za-z]+))$");
            // // somthing for society appears like [CULTURE|theStuff]

            // macro = new RuleMacro();

            // if (testforLocalConstant.Match(macroText).Success)
            // {
            //     macro.isMacro = true;
            //     macro.key = macroText;
            //     macro.society = null;
            //     return true;
            // }

            // Match match = testforCulture.Match(macroText);
            // if (match.Success)
            // {
            //     macro.isMacro = false;
            //     macro.key = match.Groups["key"].Value;
            //     macro.society = match.Groups["society"].Value;
            //     macro.society = macro.society ?? "ACTIVE"; // active society if not specified
            //     return true;
            // }

            // Log.Error($"{Globals.LOG_HEADER} Malformed rule macro: [{macroText}]");
            // macro.isMacro = false;
            // macro.key = null;
            // macro.society = null;
            // return false;
        }

    }
}
