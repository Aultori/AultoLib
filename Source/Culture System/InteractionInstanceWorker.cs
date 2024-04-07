using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class InteractionInstanceWorker
    {
        public virtual float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            return 0f;
        }

        public virtual void Interacted
                ( Pawn initiator
                , Pawn recipient
                , List<RulesetDef> extraSentencePacks
                , out string letterText
                , out string letterLabel
                , out LetterDef letterDef
                , out LookTargets lookTargets)
        {
            letterText = null;
            letterLabel = null;
            letterDef = null;
            lookTargets = null;
        }


        public virtual (string letterText, string letterLabel, LetterDef letterDef, LookTargets lookTargets)
            Interacted ( Pawn initiator, Pawn recipient, List<RulesetDef> extraSentencePacks )
        {
            return (null, null, null, null);
        }

        public InteractionInstanceDef interaction;
    }
}
