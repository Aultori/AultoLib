using AultoLib.Source.DefDefs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// Database of rules used by the Culture System sentence resolver.
    /// Also contains rules provided in InteractionDefs
    /// </summary>
    public class CultureDatabase
    {
        
        /// <summary>
        /// Makes it so pawns act differently when interacting with different cultures.
        /// </summary>
        /// <param name="category">The name of the interaction. there are many possible outcomes</param>
        /// <param name="initiator">the pawn initiating the interaction</param>
        /// <param name="recipient">the recipient pawn</param>
        /// <returns>the InteractionInstanceDef if it exists, otherwise null</returns>
        public InteractionInstanceDef GetInteractionInstanceDef ( string category, Pawn initiator, Pawn recipient )
        {
            // using strings here because the database uses strings instead of `CultureDef`s
            string initiatorCulture = CultureUtil.CultureOf(initiator).ToString();
            string recipientCulture = CultureUtil.CultureOf(recipient).ToString();
            // some variables to hold temporary values
            Dictionary<string, Dictionary<string, InteractionInstanceDef>> categoryData;
            Dictionary<string, InteractionInstanceDef> initiatorData;

            try
            {
                if (!interactionDatabase.ContainsKey(category)) throw new Exception($"interaction \"{category}\" not found");

                categoryData = interactionDatabase[category];

                if (categoryData.ContainsKey(initiatorCulture))
                {
                    // initiator found, looking for the recipiant
                    initiatorData = categoryData[initiatorCulture];
                    if (initiatorData.ContainsKey(recipientCulture)) return initiatorData[recipientCulture];
                    if (initiatorData.ContainsKey("any")) return initiatorData["any"];

                }
                if (categoryData.ContainsKey("any"))
                {
                    // initiator found, looking for the recipiant
                    initiatorData = categoryData["any"];
                    if (initiatorData.ContainsKey(recipientCulture)) return initiatorData[recipientCulture];
                    if (initiatorData.ContainsKey("any")) return initiatorData["any"];
                }
                throw new Exception($"no InteractionInstanceDef found for category='{category}' initiatorCulture='{initiatorCulture}' recipientCulture='{recipientCulture}'");
            }
            catch (Exception e)
            {
                Log.Error($"{Globals.LOG_HEADER} something went wrong: {e}");
            }
            return null;
        }

        // I want this to be done automatically, at the start, without having to call this a bunch of times, but I don't know how to do that. so I'll stick with this
        public void ProcessInteractionInstance(InteractionInstanceDef instance)
        {
            // check if the category of interaction exists
            if (!interactionDatabase.ContainsKey(instance.interactionCategory))
            {
                interactionDatabase.Add(instance.interactionCategory, new Dictionary<string, Dictionary<string, InteractionInstanceDef>>());
            }
            Dictionary<string, Dictionary<string, InteractionInstanceDef>> category = interactionDatabase[instance.interactionCategory];
            // check if the initiator culture already exists as a key
            if (!category.ContainsKey(instance.initiatorCulture))
            {
                category.Add(instance.initiatorCulture, new Dictionary<string, InteractionInstanceDef>());
            }
            // add the InteractionInstanceDef to the list
            category[instance.initiatorCulture].Add(instance.recipiantCulture, instance);
        }

        // I should never have both <initator=any,recipiant=culture> and <initiator=culture,recipiant=any> defined? one of them would always get missed???
        // <interaction type, <initator culture, <recipiant culture, Tuple<InteractionDef,InteractionMethod> > > >
        //private Dictionary<string, Dictionary<CultureDef, Dictionary<CultureDef, Tuple<InteractionDef, InteractionMethod>>>> interactionDatabase = new Dictionary<string, Dictionary<CultureDef, Dictionary<CultureDef, Tuple<InteractionDef, InteractionMethod>>>>();

        /// <summary>
        /// Dictionary&lt;Interaction name, Dictionary&lt;initiator culture, Dictionary&lt; recipient culture, List&lt;InteractionInstanceDef>>>>.
        /// using strings here instead of objects in case a CultureDef doesn't exist due to a missing mod.
        /// </summary>
        private Dictionary<string, Dictionary<string, Dictionary<string, InteractionInstanceDef>>> interactionDatabase = new Dictionary<string, Dictionary<string, Dictionary<string, InteractionInstanceDef>>>();
    }
}
