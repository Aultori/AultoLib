using AultoLib.Grammar;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Noise;

namespace AultoLib.Database
{
    public static class Ruleset_Loader
    {
        public static void Initialize(Ruleset ruleset, string society = Globals.FALLBACK_SOCIETY_KEY)
        {
            if (ruleset.initialized)
                return;

            // foreach (RulesetDef def in ruleset.includedDefs)
            // {
            //     RulesetDef_Loader.Load(def);
            //     // note, for Defs, I can keep track of which ones are loaded, then return their references
            // }

            if (ruleset.rulesStrings?.Any() == true) ruleset.Add(ExtendedRule_String_Loader.ToRuleset(ruleset.rulesStrings));
            if (ruleset.rulesFiles?.Any() == true) ruleset.Add(ExtendedRule_Word_Loader.ToRuleset(ruleset.rulesFiles, society));

            ruleset.initialized = true;
        }


        public static void LoadGlobalRuleset(Ruleset ruleset, string society)
        {
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} got here something");
            if (!GrammarDatabase.loadedSocietyRulesets.ContainsKey(society))
                GrammarDatabase.loadedSocietyRulesets[society] = new Ruleset();
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} got here adsfjagsdfjkhas");
            GrammarDatabase.loadedSocietyRulesets[society].Add(ruleset);
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} got here 2222");
        }
    }
}
