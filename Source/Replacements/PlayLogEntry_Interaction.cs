using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.Replacements
{
    /// <summary>
    /// Replaces Verse.PlayLogEntry_Interaction. Methods have slightly different parameters.
    ///
    /// This Class handles the creation of LogEntries and calls methods that generate text from InteractionDefs.
    /// 
    /// // I will need my own form of GrammerRequest
    /// </summary>
    public class PlayLogEntry_Interaction : LogEntry
    {
        /// <summary>
        /// Gets the name of the initiator pawn.
        /// returns "null" if it doesn't exist
        /// </summary>
        protected string InitiatorName
        {
            get
            {
                return (this.initiator != null)? this.initiator.LabelShort : "null";
            }
        }

        /// <summary>
        /// Gets the name of the recipiant pawn.
        /// returns "null" if it doesn't exist
        /// </summary>
        protected string RecipiantName
        {
            get
            {
                return (this.recipiant != null)? this.recipiant.LabelShort : "null";
            }
        }

        public PlayLogEntry_Interaction() : base(null)
        { }

        // I can't use CultureDefOf.fallback for default value because it needs a compile-time constant.
        // could use strings
        public PlayLogEntry_Interaction
                ( InteractionDef intDef // the interaction def
                , InteractionDef referencedIntDef // used when I want to reuse one of the default InteractionDefs
                , Pawn initiator
                , Pawn recipiant
                , CultureDef initiatorCulture
                , CultureDef recipiantCulture
                , List<RulePackDef> extraSentencePacks
                ) : base(null)
        {
            // if for some reason these fields don't have a value, fallback
            if (initiatorCulture == null) initiatorCulture = CultureDefOf.fallback;
            if (recipiantCulture == null) recipiantCulture = CultureDefOf.fallback;

            this.initiator = initiator;
        }

        // find the correct InteractionDef... no that's done ahead of this
        public PlayLogEntry_Interaction
                ( CultureInteractionDef CultIntDef // the interaction def
                , Pawn initiator
                , Pawn recipiant
                , List<RulePackDef> extraSentencePacks
                ) : base(null)
        {

        }

        // NOTE: I might eventually need my own version of RulePack

        /// <summary>
        /// The CultureInteractionDef as an InteractionDef.
        /// This InteractionDef's properties take precidence over <c>referencedInteractionDef</c>.
        /// </summary>
        private InteractionDef intDef;
        /// <summary>
        /// Used when I want to reuse an interactionDef, but have it apply to different cultures.
        /// </summary>
        private InteractionDef referencedIntDef;

        /// <summary>
        /// The initiator pawn.
        /// </summary>
        private Pawn initiator;

        /// <summary>
        /// The recipiant pawn.
        /// </summary>
        private Pawn recipiant;

    }
}
