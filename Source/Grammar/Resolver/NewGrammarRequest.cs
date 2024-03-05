using AultoLib.Grammar.Rule;
using System;
using System.Collections.Generic;
using Verse;

namespace AultoLib.Grammar.Resolver
{
    // Instead of Verse.Grammar.GrammarRequest
    public class NewGrammarRequest
    {
        public List<ExtendedRule> RulesAllowNull { get { return this.rules; } }
        public List<ExtendedRulePack> IncludesBareAllowNull { get { return this.includesBare; } }
        public List<ExtendedRulePackDef> IncludesAllowNull { get { return this.includes; } }
        public Dictionary<string, string> ConstantsAllowNull { get { return this.constants; } }

        public List<ExtendedRule> Rules {
            get
            {
                if (this.rules == null) this.rules = new List<ExtendedRule>();
                return this.rules;
            }
        }

        public List<ExtendedRulePack> IncludesBare
        {
            get
            {
                if (this.includesBare == null) this.includesBare = new List<ExtendedRulePack>();
                return this.includesBare;
            }
        }

        public List<ExtendedRulePackDef> Includes
        {
            get
            {
                if (this.includes == null) this.includes = new List<ExtendedRulePackDef>();
                return this.includes;
            }
        }

        public Dictionary<string, string> Constants
        {
            get
            {
                if (this.constants == null) this.constants = new Dictionary<string, string>();
                return this.constants;
            }
        }

        public bool HasRule(string keyword)
        {
            NewGrammarRequest request = new NewGrammarRequest();
            request.keyword = keyword;
            return ( this.rules != null
                    && this.rules.Any(new Predicate<ExtendedRule>(CS$<>8__locals1.<HasRule>g__HasTargetRule|0)))
                || ( this.includes != null
                    && this.includes.Any((ExtendedRulePackDef i) => i.RulesPlusIncludes.Any(new Predicate<Rule>(base.<HasRule>g__HasTargetRule|0))))
                || ( this.includesBare != null
                    && this.includesBare.Any(
                        (ExtendedRulePack rp) => rp.Rules.Any(new Predicate<Rule>(base.<HasRule>g__HasTargetRule|0)))
                    );
        }

        private string keyword;

        private bool TestRule(ExtendedRule rule)
        {
            return (rule.keyword == this.keyword);
        }

        public void Clear()
        {

            this.rules.Clear();
            this.includesBare.Clear();
            this.includes.Clear();
            this.constants.Clear();
        }

        private List<ExtendedRule> rules;

        private List<ExtendedRulePack> includesBare;

        private List<ExtendedRulePackDef> includes;

        private Dictionary<string, string> constants;

        public NewGrammarRequest.ICustomizer customizer;

        public interface ICustomizer
        {
            IComparer<ExtendedRule> StrictRulePrioritizer();

            void Notify_RuleUsed(ExtendedRule rule);

            bool ValidateRule(ExtendedRule rule);
            bool ValidateRule(ExtendedRule rule);
        }

    }
}
