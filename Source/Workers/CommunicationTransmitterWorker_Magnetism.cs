using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib
{
    public class CommunicationTransmitterWorker_Magnetism : CommunicationTransmitterWorker
    {
        public override bool PawnCanTransmitCommunication(Pawn pawn)
        {
            return pawn.interactions != null && this.PawnHasCapacity(pawn) && pawn.Awake() && !pawn.IsBurning();
        }

        /// <summary>
        /// If the pawn has some device/organ that directly vibrates air. <br/>
        /// Example: a speaker
        /// </summary>
        public override bool PawnHasCapacity(Pawn pawn)
        {
            return pawn.health.capacities.CapableOf(AultoLib.PawnCapacityDefOf.Magnetism);
        }
    }
}
