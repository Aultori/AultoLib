using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using RimWorld;

namespace AultoLib
{
    public static class AultoLib_PawnUtility
    {
        /// <summary>
        /// Similar to RimWorld's <see cref="RimWorld.PawnUtility.IsInteractionBlocked(Pawn, InteractionDef, bool, bool)"/> <b/>
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="category"></param>
        /// <param name="isRandomInteraction"></param>
        /// <returns></returns>
        public static bool IsCategoryBlocked(this Pawn pawn, InteractionCategoryDef category, bool isInitiator, bool isRandomInteraction = false)
        {
            // no category given. if any interaction is blocked, all categories are blocked
            if (pawn.MentalStateDef == null)
            {
                List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
                foreach (Hediff hediff in hediffs)
                {
                    if (hediff.def.blocksSocialInteraction)
                        return true;
                }
                Lord lord = pawn.GetLord();
                if ((lord?.LordJob) is LordJob_Ritual lordJob_Ritual)
                {
                    RitualRole ritualRole = lordJob_Ritual.RoleFor(pawn, true);
                    if (ritualRole?.blocksSocialInteraction ?? false)
                        return true;
                }
                return false;
            }
            if (isRandomInteraction)
            {
                return pawn.MentalStateDef.blockRandomInteraction;
            }

            if (category == null) return false;

            // look for an interactionDef with the same name as the category

            InteractionDef interaction = DefDatabase<InteractionDef>.GetNamedSilentFail(category.defName);
            if (interaction == null) return false;

            List<InteractionDef> list = isInitiator ? pawn.MentalStateDef.blockInteractionInitiationExcept : pawn.MentalStateDef.blockInteractionRecipientExcept;
            return list != null && !list.Contains(interaction);
        }
    }
}
