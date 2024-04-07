using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AultoLib.Database
{
    public class InteractionInstanceDef_Loader
    {
        public static void Load(InteractionInstanceDef interactionInstanceDef)
        {
            // initialize stuff first.
            if (interactionInstanceDef.isLoaded)
                return;

            if (interactionInstanceDef.LogRulesInitiator != null)
                interactionInstanceDef.LogRulesInitiator.Initialize();
            // Ruleset_Loader.Initialize(interactionInstanceDef.LogRulesInitiator);
            if (interactionInstanceDef.LogRulesRecipient != null)
                interactionInstanceDef.LogRulesRecipient.Initialize();
            // Ruleset_Loader.Initialize(interactionInstanceDef.LogRulesRecipient);

            // load into database
            string category = interactionInstanceDef.category;
            string initiatorSociety = interactionInstanceDef.initiatorSociety;
            string recipientSociety = interactionInstanceDef.recipientSociety;

            var categoryData = GrammarDatabase.mainInteractionInstances;
            if (!categoryData.ContainsKey(category)) categoryData[category] = new CaselessDictionary<string, CaselessDictionary<string, InteractionInstanceDef>>();
            var initiatorData = categoryData[category];
            if (!initiatorData.ContainsKey(initiatorSociety)) initiatorData[initiatorSociety] = new CaselessDictionary<string, InteractionInstanceDef>();
            var recipientData = initiatorData[initiatorSociety];
            recipientData[recipientSociety] = interactionInstanceDef;

            interactionInstanceDef.isLoaded = true;
        }
    }
}
