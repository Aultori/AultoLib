using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    /// <summary>
    /// Data used in the process of generating sentences <\br>
    /// </summary>
    public static class ResolverInstance
    {
        // instRuleset and instConstants technically don't need to be set for the resolver to work, idk why anyone wouldn't set them
        public static void Reset()
        {
            if (doLog) AultoLog.DebugMessage_Advanced("RESETTING RESOLVER");
            if (doLog) AultoLog.DebugMessage_Advanced("incrimenting runID");
            ResolverInstance.runID++;

            if (doLog) AultoLog.DebugMessage_Advanced("clearing extra tags");
            extraTagSet.Clear();


            if (doLog) AultoLog.DebugMessage_Advanced("resetting active society");
            Globals.ACTIVE_SOCIETY_KEY = Globals.FALLBACK_SOCIETY_KEY;

            // instRuleset = new Ruleset(); // Rulesets can't be cleared
            if (doLog) AultoLog.DebugMessage_Advanced("resetting instance society rulesets");
            instSocietyRulesets.Clear();

            //GrammarDatabase.loadedSocietyRulesets[Globals.INSTANCE_SOCIETY_KEY] = new Ruleset();
            if (doLog) AultoLog.DebugMessage_Advanced("clearing constances");
            instConstants.Clear();
            constantsCached = false;
        }

        public static void ClearSavedTags()
        {
            savedTagSet.Clear();
        }


        // +-----------------+
        // |    Accessors    |
        // +-----------------+
        public static Constants AllConstants
        {
            get
            {
                if (!constantsCached)
                {
                    cachedConstants.Clear();
                    cachedConstants.Add(GrammarDatabase.globalConstants);
                    cachedConstants.Add(ResolverInstance.instConstants);
                    constantsCached = true;
                }
                return cachedConstants;
            }
        }

        // +---------------+
        // |    Setters    |
        // +---------------+


        public static void SetActiveSociety(string society)
        {
            Globals.ACTIVE_SOCIETY_KEY = society;
        }

        // NOTE: rules of this instance get added to a special instance "society"
        public static void AddInstanceRuleset(Ruleset ruleset)
        {
            if (!instSocietyRulesets.ContainsKey(Globals.INSTANCE_SOCIETY_KEY))
                instSocietyRulesets[Globals.INSTANCE_SOCIETY_KEY] = new Ruleset();
            //instRuleset.Add(ruleset);
            foreach (CompoundRule compoundRule in ruleset.RulesPlusDefs.Values)
            {
                if (doLog) AultoLog.DebugMessage_Advanced("adding CompoundRule: {compoundRule.keyword}");
                instSocietyRulesets[Globals.INSTANCE_SOCIETY_KEY].Add(compoundRule);
            }
        }

        // like for INITIATOR and RECIPIENT society
        public static void AddThingRuleset(string name, Ruleset ruleset)
        {
            if (!instSocietyRulesets.ContainsKey(name))
                instSocietyRulesets[name] = new Ruleset();
            instSocietyRulesets[name].Add(ruleset);
        }

        // like for INITIATOR and RECIPIENT society
        public static void AddThingSociety(string name, string society)
        {
            if (doLog) AultoLog.DebugMessage_Advanced($"adding society {name} {society}");
            if (!instSocietyRulesets.ContainsKey(name))
                instSocietyRulesets[name] = new Ruleset();
            if (GrammarDatabase.loadedSocietyRulesets.TryGetValue(society, out Ruleset ruleset))
                instSocietyRulesets[name].Add(ruleset);
            else
            {
                if (doLog) AultoLog.DebugWarning_Advanced($"\"{society}\" not found in GrammarDatabase");
                return;
            }
        }

        public static void AddConstants(Grammar.Constants constants) => instConstants.Add(constants);
        public static void AddConstants(IEnumerable<Grammar.Constants.Constant> constants) => instConstants.Add(constants);

        public static void ExtraTags(HashSet<string> tags)
        {
            ResolverInstance.extraTagSet.Clear();
            if (tags != null)
                ResolverInstance.extraTagSet.UnionWith(tags);
        }


        // +----------------------+
        // |    Rule Validator    |
        // +----------------------+

        public static bool ValidateExtendedRule(ExtendedRule rule, HashSet<string> encounteredTags)
        {
            ProcessInstance(rule);
            return !rule.knownUnresolveable
                && ValidateConstraints(rule)
                && ValidateTags(rule, encounteredTags)
                && ValidateTimesUsed(rule);
                // && customizer?.ValidateRule(rule) == true;
        }

        private static bool ValidateConstraints(ExtendedRule rule)
        {
            if (!rule.constraintsChecked)
            {
                rule.constraintsValid = rule.Constraints == null || rule.Constraints.Test(ResolverInstance.AllConstants);
                rule.constraintsChecked = true;
            }
            return rule.constraintsValid;
        }

        // do
        // rulename[encounteredTags,[savedTag]]((requiredTag))
        private static bool ValidateTags(ExtendedRule rule, HashSet<string> encounteredTags)
        {
            if (rule.requiredTags.NullOrEmpty()) return true;

            IEnumerable<string> include = from item in rule.requiredTags
                                          where item.Value == true
                                          select item.Key;
            IEnumerable<string> exclude = from item in rule.requiredTags
                                          where item.Value == false
                                          select item.Key;

            HashSet<string> allTags = new HashSet<string>();
            allTags.AddRange(encounteredTags);
            allTags.AddRange(ResolverInstance.savedTagSet);

            // this should just be added to encounteredTags
            //    || ResolverInstance.extraTags?.Contains(rule.requiredTags.Values.) == true

            return allTags.IsProperSupersetOf(include) && !allTags.Overlaps(exclude);
        }

        private static bool ValidateTimesUsed(ExtendedRule rule)
        {
            return rule.usesLimit == null || rule.uses < rule.usesLimit;
        }


        // +------------+
        // |    Data    |
        // +------------+

        /// <summary>
        /// Resets things to keep stuff from carying over from the last time the resolver ran
        /// </summary>
        /// <returns>true if data had to be reset</returns>
        public static bool ProcessInstance( ExtendedRule rule)
        {
            if (rule.lastRunID == runID)
                return false;
            rule.lastRunID = runID;
            rule.uses = 0;
            rule.knownUnresolveable = false;
            rule.constraintsChecked = false;
            rule.constraintsValid = false;
            return true;
        }

        /// <summary>
        /// A counter that keeps track of how many times the resolver has been used.
        /// </summary>
        private static ulong runID = 0;
        /// <summary>
        /// Data describing information needed for the resolver to expand the <see cref="instRuleset"/>
        /// </summary>
        public static readonly Grammar.Constants instConstants = new Grammar.Constants();

        // /// <summary>
        // /// The <see cref="Ruleset"/> produced by the data in the <see cref="InteractionInstanceDef"/>
        // /// </summary>
        // public static Ruleset instRuleset;

        /// <summary>
        /// Rules without a society get added to a special "instance" society, which is just this instances loaded rules
        /// </summary>
        public static readonly Dictionary<string, Ruleset> instSocietyRulesets = new Dictionary<string, Ruleset> ();

        // public static string ACTIVE_SOCIETY;

        /// <summary>
        /// Tags set by something before the resolver is called
        /// </summary>
        public static readonly HashSet<string> extraTagSet = new HashSet<string>();
        /// <summary>
        /// A list to keep track of Tags between multiple runs of the resolver!
        /// </summary>
        public static readonly HashSet<string> savedTagSet = new HashSet<string>();

        public static ICustomizer customizer = null;

        private static bool constantsCached = false;
        private static readonly Grammar.Constants cachedConstants = new Grammar.Constants();

        public static bool doLog = false;
    }
}
