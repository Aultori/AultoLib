using AultoLib.Database;
using System.Collections.Generic;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// The category for a group of <see cref="InteractionInstanceDef"/>s
    /// </summary>
    public class InteractionCategoryDef : Def
    {
        public override void PostLoad()
        {
            // add to GrammarDatabase so when I try to resolve the references of InteractionInstanceDefs, it can be added to the category.
            // it's also usefull for checking if a category exists, by looking for the def

            GrammarDatabase.mainInteractionInstances[this.defName] = new CaselessDictionary<string, CaselessDictionary<string, InteractionInstanceDef>>();
        }

        public bool ignoreTimeSinceLastInteraction = false;
    }
}
