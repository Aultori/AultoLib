using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Verse;
using AultoLib.Database;

namespace AultoLib.Grammar
{
    public class Ruleset
    {
        public Ruleset() { }

        public Dictionary<string, CompoundRule> RulesInitialized
        {
            get
            {
                if (this.rulesStrings == null && this.rulesFiles == null) return this.cachedRules.Rules;

                // not needed because of the rules in the ResolverInstance
                // if (this.useActiveSociety)
                // {
                //     Ruleset_Loader.Initialize(this, ResolverInstance.ACTIVE_SOCIETY);
                // } else
                if (!this.initialized)
                {
                    this.Initialize();
                }

                return this.cachedRules.Rules;
            }
        }

        /// <summary>
        /// Rules obtained through included RulesetDefs
        /// </summary>
        public Dictionary<string, CompoundRule> RulesResolved
        {
            get
            {
                if (this.resolvedRules == null)
                {
                    this.resolvedRules = new RuleCollection();
                    if (this.includedDefs != null) this.ResolveReferences();
                }
                return this.resolvedRules.Rules;
            }
        }

        public Dictionary<string, CompoundRule> RulesPlusDefs
        {
            get
            {
                if (this.rulesStrings == null && this.rulesFiles == null) return this.cachedRules.Rules;

                if (this.rulesPlusDefs == null)
                { 
                    this.rulesPlusDefs = new RuleCollection();
                    this.rulesPlusDefs.AddDict(this.RulesInitialized);
                    this.rulesPlusDefs.AddDict(this.RulesResolved);
                }

                return this.rulesPlusDefs.Rules;
            }
        }

        public Dictionary<string, CompoundRule> RulesFromCategories(string society_key)
        {
            var tmp = new RuleCollection();
            foreach (string category in this.includedCategories)
            {
                if (GrammarDatabase.TryGetRulesetDef(society_key, category, out RulesetDef def))
                    tmp.Add(def);
            }
            return tmp.Rules;
        }

        public Dictionary<string, CompoundRule> AllRules(string society_key)
        {
            var tmp = new RuleCollection();
            tmp.AddDict(this.RulesPlusDefs);
            // tmp.AddRange(this.RulesFromCategories(society_key));
            foreach (CompoundRule compoundRule in this.RulesFromCategories(society_key).Values)
               tmp.Add(compoundRule);

            // if (!this.cachedRules.ContainsKey(rule.keyword))
            //     cachedRules.Add(rule.keyword, new CompoundRule(rule.keyword));

            // this.cachedRules[rule.keyword].Add(rule);
            return tmp.Rules;
        }

        public bool TryGetCompoundRule(string name, out CompoundRule compoundRule)
        {
            // compoundRule = null;
            // if (!this.initialized) return false;
            if (this.cachedRules.TryGetValue(name, out compoundRule)) return true;
            return false;
        }

        public bool TryGetFirstKeyword(out string keyword)
        {
            if (TryFindFirstKeyword(this.rulesStrings, out keyword)) return true;
            if (TryFindFirstKeyword(this.rulesFiles, out keyword)) return true;
            keyword = null;
            return false;
        }

        private static bool TryFindFirstKeyword(List<string> rawRules, out string keyword)
        {
            if (rawRules?[0] != null)
            {
                Match match = keywordPattern.Match(rawRules[0]);
                if (match.Success)
                {
                    keyword = match.Value;
                    return true;
                }
            }
            keyword = null;
            return false;
        }

        // public bool TryGetRulesetFromCategories(string society, out Ruleset ruleset)
        // {
        //     ruleset = new Ruleset();
        //     if (this.includedCategories?.Any() == false) return false;
        //     foreach (string category in this.includedCategories)
        //     {
        //         if (GrammarDatabase.TryGetRulesetDef(society, category, out RulesetDef rulesetDef))
        //             ruleset.Add(rulesetDef);
        //     }
        //     return true;
        // }

        // +----------------------------------+
        // |    Methods that modify things    |
        // +----------------------------------+

        // public void Add(ExtendedRule rule)
        // {
        //     if (!CanEdit()) return;
        //     if (!this.cachedRules.ContainsKey(rule.keyword))
        //         cachedRules.Add(rule.keyword, new CompoundRule(rule.keyword));
        //     this.cachedRules[rule.keyword].Add(rule);
        // }

        // public void Add(CompoundRule compoundRule)
        // {
        //     if (!CanEdit()) return;
        //     if (!this.cachedRules.ContainsKey(compoundRule.keyword))
        //         cachedRules.Add(compoundRule.keyword, new CompoundRule(compoundRule.keyword));
        //     this.cachedRules[compoundRule.keyword].Add(compoundRule);
        // }

        // public void Add(Ruleset ruleset)
        // {
        //     if (!CanEdit()) return;
        //     // foreach (CompoundRule compoundRule in ruleset.cachedRules.Values)
        //     foreach (CompoundRule compoundRule in ruleset.RulesPlusDefs.Values)
        //        this.Add(compoundRule);
        //     // I can't do this, or duplicate elements can get added
        //     // this.cachedRules.AddRange(ruleset.RulesPlusDefs);
        // }

        // public void Add(RulesetDef rulesetDef)
        // {
        //     if (!CanEdit()) return;
        //     // this.cachedRules.AddRange(rulesetDef.ruleset.RulesPlusDefs);
        //     this.Add(rulesetDef.ruleset);
        // }

        public void Add(ExtendedRule rule)
        {
            if (!CanEdit()) return;
            this.cachedRules.Add(rule);
        }
        public void Add(CompoundRule compoundRule)
        {
            if (!CanEdit()) return;
            this.cachedRules.Add(compoundRule);
        }
        public void Add(Ruleset ruleset)
        {
            if (!CanEdit()) return;
            this.cachedRules.Add(ruleset);
        }
        public void Add(RulesetDef rulesetDef)
        {
            if (!CanEdit()) return;
            this.cachedRules.Add(rulesetDef);
        }
        public void AddDict(Dictionary<string, CompoundRule> dict)
        {
            if (!CanEdit()) return;
            this.cachedRules.AddDict(dict);
        }

        public void ResolveReferences()
        {
            if (Logging.DoLog()) Logging.Message($"resolving references");
            if (this.rulesStrings == null && this.rulesFiles == null)
            {
                Logging.Error($"Cannot resolve the references of a Ruleset with no {nameof(rulesStrings)} or {nameof(rulesFiles)} to resolve");
                return;
            }
            // loads things into rulesPlusDefs
            this.resolvedRules = new RuleCollection();
            foreach (RulesetDef rulesetDef in this.includedDefs)
            {
                this.resolvedRules.AddDict(rulesetDef.ruleset.RulesInitialized); // useActiveSociety will never be set
                this.resolvedRules.AddDict(rulesetDef.ruleset.RulesResolved);
            }
            this.rulesPlusDefs = new RuleCollection();
            this.rulesPlusDefs.AddDict(this.RulesInitialized); // initializes cached rules if there isn't anything there
            this.rulesPlusDefs.Add(this.resolvedRules);
            this.MakeReadonly();

        }

        public void Initialize()
        {
            if (!this.initialized)
            {
                // Ruleset_Loader.Initialize(this, this.societySetByDef ?? Globals.FALLBACK_SOCIETY_KEY);
                if (this.rulesStrings?.Any() == true) this.Add(ExtendedRule_String_Loader.ToRuleset(this.rulesStrings));
                if (this.rulesFiles?.Any() == true) this.Add(ExtendedRule_Word_Loader.ToRuleset(this.rulesFiles, this.societySetByDef ?? Globals.FALLBACK_SOCIETY_KEY));

                this.initialized = true;
            }

        }

        public void MakeReadonly() => this.isReadonly = true;

        private bool CanEdit()
        {
            // if (this.rulesStrings != null || this.rulesFiles != null)
            if (this.isReadonly)
            {
                Logging.Error("Cannot add to a Ruleset that's set to readonly");
                return false;
            }
            return true;
        }

        // +---------------+
        // |    Copying    |
        // +---------------+

        // public Ruleset DeepCopy()
        // {
        //     Ruleset pack = new Ruleset
        //     {
        //         cachedRules = this.cachedRules.ToDictionary(e => e.Key, e => e.Value.DeepCopy()),
        //         initialized = this.initialized,
        //     };
        //     return pack;
        // }
        // public Ruleset LightCopy()
        // {
        //     Ruleset pack = new Ruleset
        //     {
        //         cachedRules = this.cachedRules.ToDictionary(e => e.Key, e => e.Value.LightCopy()),
        //         initialized = this.initialized,
        //     };
        //     return pack;
        // }

        // +-----------------+
        // |    Variables    |
        // +-----------------+

        // // not needed because includedCategories only appears in InteractionInstanceDef, and that already has the society defined
        // actually needed because there's two societies
        // not needed because there's two societies and there isn't a good one to choose from.
        // public string society; // will be used by RulesetDef?

        public List<RulesetDef> includedDefs;
        // categories of ExtendedRulePackDefs
        // do I need this?
        // yes, but the method that loads this is given a society first.
        public List<string> includedCategories; 

        public List<string> rulesStrings;
        public List<string> rulesFiles;

        [Unsaved(false)] public bool initialized = false;
        [Unsaved(false)] private bool isReadonly = false;
        [Unsaved(false)] public string societySetByDef; // set by the ruleset def
        [Unsaved(false)] private RuleCollection cachedRules = new RuleCollection(); 
        [Unsaved(false)] private RuleCollection resolvedRules; 
        [Unsaved(false)] private RuleCollection rulesPlusDefs; 

        [Unsaved(false)] private static readonly Regex keywordPattern = new Regex("^[a-zA-Z0-9_/]+");

        public class RuleCollection
        {
            public Dictionary<string, CompoundRule> Rules => this.data;

            public void Add(ExtendedRule rule)
            {
                if (!this.data.ContainsKey(rule.keyword))
                    // add new key if it doesn't exist
                    data[rule.keyword] = new CompoundRule(rule.keyword);

                this.data[rule.keyword].Add(rule);
            }

            public void Add(CompoundRule compoundRule)
            {
                if (!this.data.ContainsKey(compoundRule.keyword))
                    data[compoundRule.keyword] = new CompoundRule(compoundRule.keyword);

                this.data[compoundRule.keyword].Add(compoundRule);
            }

            public void Add(Ruleset ruleset)
            {
                foreach (CompoundRule compoundRule in ruleset.RulesPlusDefs.Values)
                   this.Add(compoundRule);
                // I can't do this, or duplicate elements can get added
                // this.data.AddRange(ruleset.RulesPlusDefs);
            }

            public void Add(RulesetDef rulesetDef)
            {
                this.Add(rulesetDef.ruleset);
            }

            public void Add(RuleCollection ruleCollection)
            {
                foreach (CompoundRule compoundRule in ruleCollection.data.Values) this.Add(compoundRule);
            }

            public void AddDict(Dictionary<string, CompoundRule> dict)
            {
                foreach (CompoundRule compoundRule in dict.Values) this.Add(compoundRule);
            }

            public bool TryGetValue(string key, out CompoundRule value) => this.data.TryGetValue(key, out value);

            private Dictionary<string, CompoundRule> data = new Dictionary<string, CompoundRule>();
        }
    }
}
