using System;
using System.Collections.Generic;
using System.Linq;
//using Verse.Grammar;
using RimWorld;
using AultoLib.Grammar;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// Replacement for Verse.Grammar.RulePack
    /// </summary>
    public class CulturalRulePack
    {
        public List<RuleExtended> Rules
        {
            get
            {
                if (rulesResolved == null)
                {
                    rulesResolved = CulturalRulePack.GetRulesResolved(rulesRaw, rulesStrings, rulesFiles);
                    if (include != null)
                    {
                        foreach (RulePackDef rulePackDef in include)
                        {
                            rulesResolved.AddRange(rulePackDef.RulesPlusIncludes);
                        }
                    }
                }
                return rulesResolved;
            }
        }

        public List<string> RuleFiles { get { return rulesFiles; } }

        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        public List<string> rulesStrings = new List<string>();

        public List<string> rulesFiles = new List<string>();

        private List<RuleExtended> rulesRaw;

        public List<CulturalRulePackDef> include;

        [Unsaved(false)]
        private List<RuleExtended> rulesResolved;

        [Unsaved(false)]
        private List<RuleExtended> untranslatedRulesResolved;

        [Unsaved(false)]
        private List<string> untranslatedRulesStrings;

        [Unsaved(false)]
        private List<string> untranslatedRulesFiles;

        [Unsaved(false)]
        private List<RuleExtended> untranslatedRulesRaw;
    }
}
