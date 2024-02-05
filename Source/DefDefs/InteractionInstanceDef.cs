using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AultoLib
{
    public class InteractionInstanceDef : Def
    {
        /// <summary>
        /// should be the label of the InteractionDef?
        /// the kind of interation, the group.
        /// not an individual outcome, but the name of the set of outcomes
        /// </summary>
        public string interactionCategory;

        /// <summary>
        /// The InteractionDef to select if the cultures match.
        /// </summary>
        public InteractionDef interactionDef;

        // these should actually be strings because some mods might have extra interactions for cultures the user doesn't have the mod for.
        // If that culture doesn't exist, just ignore it.
        public string initiatorCulture; // self explanitory
        public string recipiantCulture; // self explanitory

        /// <summary>
        /// Interaction method classes. Talking, body language, things like that.
        /// The first one in the list will be checked for first.
        /// </summary>
        public List<CommunicationMethod> interactionMethods;
    }
}

