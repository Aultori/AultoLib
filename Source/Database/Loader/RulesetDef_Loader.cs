using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AultoLib.Grammar;
using Verse;

namespace AultoLib.Database
{
    public static class RulesetDef_Loader
    {
        // public static void InitializeRulesetDef(RulesetDef def)
        // {
        //     if (def.initialized) return;
        //     if (def.cachedRuleset != null) return;
        //     if (def.ruleset?.initialized == false)
        //     {
        //         Log.Error($"{Globals.LOG_HEADER} tried to initialize a RulesetDef with an uninitialized Ruleset.");
        //         return;
        //     }

        //     def.cachedRuleset = new Ruleset();
        //     def.cachedRuleset.Add(def.ruleset);
        //     foreach (RulesetDef def2 in def.includedDefs)
        //     {
        //         // initialize each ruleset
        //         RulesetDef_Loader.Load(def2);
        //         def.cachedRuleset.Add(def2.AllRules);
        //     }
        //     return;
        // }

        // public static void Load(RulesetDef rulesetDef)
        // {
        //     // it will be configured properly due to the `ConfigErrors()` method

        //     if (rulesetDef.initialized) return;
        //     Ruleset_Loader.Initialize(rulesetDef.ruleset);
        //     RulesetDef_Loader.InitializeRulesetDef(rulesetDef);
        //     LoadToDatabase(rulesetDef.society, rulesetDef.category, rulesetDef);

        //     rulesetDef.initialized = true;
        // }

        /// <summary>
        /// Loads a RulesetDef into the GrammarDatabase
        /// </summary>
        public static void LoadToDatabase(string society, string category, RulesetDef def)
        {
#if DEBUG
            Log.Message($"{Globals.DEBUG_LOG_HEADER} Loaded RulesetDef: {society}-->{category}-->{def.defName}");
#endif
            var societyData = GrammarDatabase.loadedRulesetDefs;
            if (!societyData.ContainsKey(society))
            {
                societyData[society] = new Dictionary<string, RulesetDef>();
            }
            var categoryData = societyData[society];

            societyData[society][category] = def;
        }
    }
}
