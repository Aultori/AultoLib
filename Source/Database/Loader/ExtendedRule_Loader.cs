using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AultoLib.Grammar;
using Verse;

namespace AultoLib.Database
{
    public static class ExtendedRule_Loader
    {

        private static void CheckAssignment(string name, string op, string rawString)
        {
            if (op != "=") Log.Error($"{Globals.LOG_HEADER} Attempt to compare {name} instead of assigning in rule {rawString}");
        }

        public static bool TryBuildRule(string rawString, ExtendedRule rule, out string contents)
        {
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} trying to build rule: {rawString}");

            Match match = ExtendedRule_Loader.aultoLibPattern.Match(rawString);

            if (!match.Success)
            {
                Log.Error($"{Globals.LOG_HEADER} Bad string pass when reading rule {rawString}");
                contents = null;
                return false;
            }

            rule.keyword = match.Groups["keyword"].Value;

            contents = match.Groups["output"].Value;
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} keyword = {match.Groups["keyword"].Value}, contents = {match.Groups["output"].Value}");

            // Log.Message($"{Globals.DEBUG_LOG_HEADER} keyword = {rule.keyword}, contents = {contents}");

            for (int i = 0; i < match.Groups["paramname"].Captures.Count; i++)
            {
                string name = match.Groups["paramname"].Captures[i].Value;
                string op = match.Groups["paramoperator"].Captures[i].Value;
                string value = match.Groups["paramvalue"].Captures[i].Value;

                switch (name)
                {
                case "p":
                    CheckAssignment(name, op, value);
                    rule.weight = float.Parse(value);
                    break;
                case "priority":
                    CheckAssignment(name, op, value);
                    rule.priority = float.Parse(value);
                    break;
                case "tag":
                    CheckAssignment(name, op, value);
                    Log.Warning($"{Globals.LOG_HEADER} use of 'tag' is depricated");
                    break;
                case "requiredTag":
                    CheckAssignment(name, op, value);
                    Log.Warning($"{Globals.LOG_HEADER} use of 'requiredTag' is depricated");
                    break;
                case "uses":
                    CheckAssignment(name, op, value);
                    rule.usesLimit = new int?(int.Parse(value));
                    break;
                case "debug":
                    #if !DEBUG
                    Log.Error($"{Globals.LOG_HEADER} Rule '{value}' contains debug flag; fix before commit");
                    #endif
                    break;
                default:
                    rule.constraintList.AddConstraint(name, op, value);
                    break;
                }
            }

            for (int i= 0; i < match.Groups["tag"].Captures.Count; i++)
            {
                // truthy: add tag. !truthy: remove tag
                string tag = match.Groups["tag"].Captures[i].Value;
                bool value = (match.Groups["tagrem"].Captures[i].Value == "-") ? false : true; // I could simplify these
                if (rule.tags == null) rule.tags = new Dictionary<string, bool>();
                rule.tags[tag] = value;
            }
            for (int i= 0; i < match.Groups["savedtag"].Captures.Count; i++)
            {
                // truthy: add tag. !truthy: remove tag
                string savedtag = match.Groups["savedtag"].Captures[i].Value;
                bool value = (match.Groups["s_tagrem"].Captures[i].Value == "-") ? false : true;
                if (rule.savedTags == null) rule.savedTags = new Dictionary<string, bool>();
                rule.savedTags[savedtag] = value;
            }
            for (int i= 0; i < match.Groups["requiredtag"].Captures.Count; i++)
            {
                // truthy: needs tag. !truthy: must not have that tag
                string requiredtag = match.Groups["requiredtag"].Captures[i].Value;
                bool value = (match.Groups["r_taginv"].Captures[i].Value == "!") ? false : true;
                if (rule.requiredTags == null) rule.requiredTags = new Dictionary<string, bool>();
                rule.requiredTags[requiredtag] = value;
            }

            return true;
        }

        // The regex pattern from RimWorld
        private static readonly Regex pattern = new Regex(""
 + "		# hold on to your butts, this is gonna get weird"
 + "		^"
 + "		(?<keyword>[a-zA-Z0-9_/]+)					# keyword; roughly limited to standard C# identifier rules"
 + "		(											# parameter list is optional, open the capture group so we can keep it or ignore it"
 + "			\\(										# this is the actual parameter list opening"
 + "				(									# unlimited number of parameter groups"
 + "                    [ ]*                            # leading whitespace for readability"
 + "					(?<paramname>[a-zA-Z0-9_/]+)	# parameter name is similar"
 + "					(?<paramoperator>==|=|!=|>=|<=|>|<|) # operators; empty operator is allowed"
 + "					(?<paramvalue>[^\\,\\)]*)			# parameter value, however, allows everything except comma and closeparen!"
 + "					,?								# comma can be used to separate blocks; it is also silently ignored if it's a trailing comma"
 + "				)*"
 + "			\\)"
 + "		)?"
 + "        [ ]*                                        # leading whitespace before -> for readability"
 + "		->(?<output>.*)								# output is anything-goes"
 + "		$"
 + "		", RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

        // moddified regex pattern
        private static Regex aultoLibPattern = new Regex(""
 + "		^"
 + "		(?<keyword>[a-zA-Z0-9_/]+)					"
 + "        (?: " // the encountered tag list   
 + "            \\[ "   
 + "            [ ]* "   
 + "            (?: "   
 + "                (?: "   
 + "                    (?<tagrem>[-]?)(?<tag>[a-zA-Z0-9_/]+) "   // encountered tag 
 + "                | "   
 + "                    \\[ (?<s_tagrem>[-]?)(?<savedtag>[a-zA-Z0-9_/]+) \\] "   // tag to save 
 + "                    "   
 + "                ) "   
 + "                "   
 + "            ,?[ ]* )* "   
 + "            \\] "   
 + "        )? " 
 + "		(											"
 + "			\\(										" // normal parameter list
 + "				(?:									"
 + "                    [ ]*                            " 
 + "                    "   
 + "                    (?: " // some sort of parameter:   
 + "                        (?: " 
 + "                            \\( (?<r_taginv>!|)(?<requiredtag>[a-zA-Z0-9_/]+)   \\) "   
 + "                        ) "   
 + "                        | "  
 + "                        (?: " // regular parameter
 + "					        (?<paramname>[a-zA-Z0-9_/]+)	 "
 + "					        (?<paramoperator>==|=|!=|>=|<=|>|<|)  "
 + "					        (?<paramvalue>[^,()]*)			 "
 + "                        ) "
 + "                    )"   
 + "					,?								 "
 + "				)*"
 + "			\\)"
 + "		)?"
 + "        [ ]*                                         "
 + "		->(?<output>.*)								 "
 + "		$"
 + "		", RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
    }
}
