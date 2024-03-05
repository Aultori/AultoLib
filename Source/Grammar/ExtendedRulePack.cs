using System;
using System.Collections.Generic;
using System.Linq;
//using Verse.Grammar;
using RimWorld;
using AultoLib.Grammar;
using Verse;
using Steamworks;
using AultoLib.Grammar.Macro;

namespace AultoLib.Grammar
{
    /// <summary>
    /// Replacement for Verse.Grammar.RulePack
    /// </summary>
    public class ExtendedRulePack
    {
        public List<ExtendedRule> Rules
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public List<ExtendedRule> UntranslatedRules
        {
            get 
            {
                throw new NotImplementedException(); 
            }
        }

        public void PostLoad()
        {
            throw new NotImplementedException();
        }


        


        public List<string> RuleFiles { get { return rulesFiles; } }

        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        public List<string> rulesStrings = new List<string>();

        public List<string> rulesFiles = new List<string>();

        private List<ExtendedRule> rulesRaw;

        public List<ExtendedRulePackDef> include;

        [Unsaved(false)]
        private List<ExtendedRule> rulesResolved;

        [Unsaved(false)]
        private List<ExtendedRule> untranslatedRulesResolved;

        [Unsaved(false)]
        private List<string> untranslatedRulesStrings;

        [Unsaved(false)]
        private List<string> untranslatedRulesFiles;

        [Unsaved(false)]
        private List<ExtendedRule> untranslatedRulesRaw;
    }
}
