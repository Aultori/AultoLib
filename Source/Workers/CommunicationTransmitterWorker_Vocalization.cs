using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class CommunicationTransmitterWorker_Vocalization : CommunicationTransmitterWorker
    {
        // public static bool PawnCanInitiate(Pawn pawn, InteractionCategoryDef category)
        // {
        //     return pawn.interactions != null && pawn.health.capacities.CapableOf(RimWorld.PawnCapacityDefOf.Talking) && pawn.Awake() && !pawn.IsBurning() && !pawn.IsCategoryBlocked(category, true, false);
        // }

        // I think I shouldn't have InteractionCategoryDef here
        public override bool PawnCanTransmitCommunication(Pawn pawn)
        {
            return pawn.interactions != null && this.PawnHasCapacity(pawn) && pawn.Awake() && !pawn.IsBurning();
        }

        public override bool PawnHasCapacity(Pawn pawn)
        {
            return pawn.health.capacities.CapableOf(RimWorld.PawnCapacityDefOf.Talking);
        }
    }
}
