using System.Collections.Generic;
using Verse;
using AultoLib.Grammar;
using AultoLib.Database;

namespace AultoLib
{
    public class RulesetDef : Def
    {
        public string FirstKeyword
        {
            get
            {
                if (this.ruleset?.TryGetFirstKeyword(out string key) == true)
                    return key;
                return null;
            }
        }


        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
                yield return error;

            if (this.society == null) yield return "society is null";
            if (this.category == null) yield return "category is null";
            if (this.ruleset == null)                       yield return "ruleset is null";
            if (this.ruleset.includedCategories != null)    yield return "the field ruleset has includedCategories set";
            if (this.ruleset.includedDefs == null)          yield break; // end early. nothing after this needs to be checked
            if (this.ruleset.includedDefs.Contains(this))   yield return "contains itself";

            foreach (RulesetDef includedDef in this.ruleset.includedDefs)
                if (includedDef.ruleset.includedDefs?.Contains(this) == true)  yield return $"includes other RulesetDef which includes it: {includedDef.defName}";

            yield break;
        }

        // add to database
        public override void ResolveReferences()
        {
            this.ruleset.societySetByDef = society.ToUpper();
            RulesetDef_Loader.LoadToDatabase(this.society, this.category, this);
        }

        // public override void ResolveReferences()
        // {
        //     ruleset.ResolveReferences();
        // }


        public static RulesetDef Named(string defName)
        {
            return DefDatabase<RulesetDef>.GetNamed(defName);
        }
        // +-----------------+
        // |    Variables    |
        // +-----------------+

        public string society;
        public string category;

        // public List<RulesetDef> includedDefs;

        // include defs using the `includedDefs` field in Ruleset
        public Ruleset ruleset;

        // will always be loaded
    }
}
