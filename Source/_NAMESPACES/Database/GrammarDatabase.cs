using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using AultoLib.Grammar;
using Steamworks;
using Verse;

namespace AultoLib.Database
{
    /// <summary>
    /// This class keeps track of all rules used by the Society System.
    /// It provides utilites for accessing those rules.
    /// </summary>
    public static class GrammarDatabase
    {
        /// <summary>
        /// Makes it so pawns act differently depending on their society!
        /// the society defined in the InteractionInstanceDef can be "any", which will be chosen if the desired society isn't found.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="initiatorSociety"></param>
        /// <param name="recipientSociety"></param>
        // After selecting, make sure to check if the pawns have the capacities to do the interaction: the communicaiton methods
        public static bool TryGetInteractionInstance(string category, string initiatorSociety, string recipientSociety, out InteractionInstanceDef inter)
        {
            if (mainInteractionInstances.TryGetValue(category, out var initiatorData))
            {
                if (initiatorData.TryGetValue(initiatorSociety.ToLower(), out var recipientData))
                {
                    // Initiator society found
                    if (recipientData.TryGetValue(recipientSociety.ToLower(), out inter)) return true;
                    if (recipientData.TryGetValue("any", out inter)) return true;
                }
                // nothing for Recipient society found, fallback to "any", then try again
                if (initiatorData.TryGetValue("any", out recipientData))
                {
                    if (recipientData.TryGetValue(recipientSociety.ToLower(), out inter)) return true;
                    if (recipientData.TryGetValue("any", out inter)) return true;
                }
            }
            AultoLog.Error($"InteractionInstanceDef not found in {category}-->{initiatorSociety}-->{recipientSociety}.");
            inter = null;
            return false;
        }
        // public static bool TryGetInteractionInstance(string category, SocietyDef initiatorSocietyDef, SocietyDef recipientSocietyDef, out InteractionInstanceDef inter) => TryGetInteractionInstance(category, initiatorSocietyDef, recipientSocietyDef)

        public static bool TryGetRulesetDef(string society, string category, out RulesetDef def)
        {
            def = null;
            var societyData = mainRulesetDefs;
            // check if the keys exist
            if (societyData.TryGetValue(society, out var categoryData))
            {
                if (categoryData.TryGetValue(category, out def)) return true;
                // no category found, look in fallback
            }
            if (societyData.TryGetValue(Globals.FALLBACK_SOCIETY_KEY, out categoryData))
            {
                if (categoryData.TryGetValue(category, out def)) return true;
                // no category found. this is an error
                AultoLog.Error($"The category {category} was not found in the fallback society's loadedRulesetDefs");
                return false;
            }
            AultoLog.Error($"The society {society} does not exist in loadedRulesetDefs.");
            return false;
        }


        public static bool TryGetConstant(string KEY, out string value)
        {
            // if (ResolverInstance.instConstants.TryGetValue(KEY, out value)) return true;
            // if (GrammarDatabase.globalConstants.TryGetValue(KEY, out value)) return true;
            if (ResolverInstance.AllConstants.TryGetValue(KEY, out value)) return true;
            AultoLog.Error($"Constant [{KEY}] not found!");
            return false;
        }

        // public static bool TryGetLocalCompoundRule(string KEY, out CompoundRule rule)
        // {
        //     if (ResolverInstance.instRuleset.TryGetCompoundRule(KEY, out rule)) return true;
        //     return false;
        // }

        public static bool TryGetGlobalCompoundRule(string society, string key, out CompoundRule rule)
        {
            if (GrammarDatabase.mainSocietyRulesets.TryGetValue(society, out Ruleset ruleset))
            {
                if (ruleset.TryGetCompoundRule(key, out rule)) return true;
            }
            // if (GrammarDatabase.globalUtilityRulesets.TryGetValue(society, out ruleset))
            // {
            //     if (ruleset.TryGetCompoundRule(key, out rule)) return true;
            // }
            if (GrammarDatabase.mainSocietyRulesets.TryGetValue(Globals.FALLBACK_SOCIETY_KEY, out ruleset))
            {
                if (ruleset.TryGetCompoundRule(key, out rule)) return true;
                AultoLog.Error($"key {key} not found in the fallback society's ruleset");
                return false;
            }
            AultoLog.Error($"no fallback society found in societyRuleset");
            rule = null;
            return false;
        }

        // /// <summary>
        // /// searches the resolver instance and the database for a <see cref="CompoundRule"/>.
        // /// The string <paramref name="society"/> can be null
        // /// </summary>
        // /// <param name="society">can be null</param>
        // /// <param name="key"></param>
        // /// <param name="rule"></param>
        // /// <returns></returns>
        // public static bool TryGetCompoundRule(string society, string key, out CompoundRule rule)
        // {
        //     // try to find the macro
        //     // could be in the local ruleset
        //     // could be in the global ruleset
        //     if (society.NullOrEmpty())
        //     {
        //         // look in local ruleset
        //         if (ResolverInstance.instRuleset.TryGetCompoundRule(key, out rule))
        //             return true;
        //     }
        //     // look in global ruleset
        //     string society2 = society ?? ResolverInstance.ACTIVE_SOCIETY;
        //     if (GrammarDatabase.TryGetGlobalCompoundRule(society2, key, out rule))
        //         return true;
        //     // no macro found
        //     rule = null;
        //     return false;
        // }

        /// <summary>
        /// searches the instance and the database for a <see cref="CompoundRule"/>.
        /// The string <paramref name="society"/> can be null
        /// </summary>
        /// <param name="society">can be null</param>
        /// <param name="key"></param>
        /// <param name="rule"></param>
        /// <returns></returns>
        public static bool TryGetCompoundRule(string society, string ruleKey, out CompoundRule rule)
        {
            // check for it in the instance
            if (ResolverInstance.instSocietyRulesets.TryGetCompoundRuleFromDict(society, ruleKey, out rule)) return true;

            // check for it in the global rulesets
            if (GrammarDatabase.mainSocietyRulesets.TryGetCompoundRuleFromDict(society, ruleKey, out rule)) return true;

            // society has a chance of being Global.INSTANCE_SOCIETY_KEY, so check the active society if it was
            if (society == Globals.INSTANCE_SOCIETY_KEY)
            {
                if (ResolverInstance.instSocietyRulesets.TryGetCompoundRuleFromDict(Globals.ACTIVE_SOCIETY_KEY, ruleKey, out rule)) return true;
                if (GrammarDatabase.mainSocietyRulesets.TryGetCompoundRuleFromDict(Globals.ACTIVE_SOCIETY_KEY, ruleKey, out rule)) return true;
            }

            Log.Warning($"{Globals.LOG_HEADER} society \"{society}\" not found");

            // allways check the fallback
            if (GrammarDatabase.mainSocietyRulesets.TryGetCompoundRuleFromDict(Globals.FALLBACK_SOCIETY_KEY, ruleKey, out rule)) return true;
            
            // no macro found
            AultoLog.Error($"key \"{ruleKey}\" not found in the fallback society's ruleset");
            rule = null;
            return false;
        }

        private static bool TryGetCompoundRuleFromDict(this Dictionary<string, Ruleset> dict, string society, string ruleKey, out CompoundRule rule)
        {
            rule = null;
            if (!dict.TryGetValue(society, out Ruleset ruleset)) return false;
            if (!ruleset.TryGetCompoundRule(ruleKey, out rule)) return false;
            return true;
        }

        // +------------------+
        // |    Extentions    |
        // +------------------+

        /// <summary>
        /// Gets the rules derived from the included categories, but only that. Doesn't return the regular rules.
        /// </summary>
        /// <returns></returns>
        public static bool TryGetRulesetFromCategories(this Ruleset r, string society, out Ruleset ruleset)
        {
            ruleset = new Ruleset();

            if (r.includedCategories?.Any() == false) return false;

            foreach (string category in r.includedCategories)
            {
                if (GrammarDatabase.TryGetRulesetDef(society, category, out RulesetDef rulesetDef))
                    ruleset.Add(rulesetDef);
            }

            return true;
        }

        // +------------------------+
        // |     The Variables      |
        // +------------------------+

        // InteractionInstance data
        // category --> initiator society --> recipient society --> communication method --> InteractionInstance

        /// <summary>
        /// <b>InteractionInstance data:</b>
        /// category --> initiator society --> recipient society --> InteractionInstanceDef
        /// </summary>
        internal static Dictionary<string, CaselessDictionary<string, CaselessDictionary<string, InteractionInstanceDef>>>
            mainInteractionInstances = new Dictionary<string, CaselessDictionary<string, CaselessDictionary<string, InteractionInstanceDef>>>();

        /// <summary>
        /// Loaded RulesetDefs. The categories for them.
        /// society --> category --> def
        /// </summary>
        internal static CaselessDictionary<string, Dictionary<string, RulesetDef>>
            mainRulesetDefs = new CaselessDictionary<string, Dictionary<string, RulesetDef>>();

        // society --> path --> rawText
        internal static CaselessDictionary<string, Dictionary<string, string>>
            loadedTextFiles = new CaselessDictionary<string, Dictionary<string, string>>();

        // society key --> SocietyDef
        internal static CaselessDictionary<string,SocietyDef>
            loadedSocietyDefs = new CaselessDictionary<string,SocietyDef>();

        // internal static Grammar.Constants localConstants = new Grammar.Constants();


        // internal static Ruleset localRuleset = new Ruleset();

        internal static Grammar.Constants globalConstants = new Grammar.Constants();

        /// <summary>
        /// society key --> ruleset
        /// </summary>
        internal static CaselessDictionary<string, Ruleset> mainSocietyRulesets = new CaselessDictionary<string, Ruleset>();


        // /// <summary>
        // /// Local rule list. for things like RECIPIENT_name
        // /// maybe I could combine these and the constraints that get passed around.
        // /// </summary>
        // // prefix --> suffix --> value
        // internal static Dictionary<string, Dictionary<string, string>> miniRule = new Dictionary<string, Dictionary<string, string>>();
        //
        // // make a thing for constraints.
        // // call them local values instead of constraints. 
        // internal static Dictionary<string, string> localValues = new Dictionary<string, string>();

    }
}
