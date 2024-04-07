using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib
{
    public class CommunicationReceiverWorker_EMReceiver : CommunicationReceiverWorker
    {
        public override bool PawnCanRecieveCommunication(Pawn pawn)
        {
            return pawn.Awake() && !pawn.IsBurning();
        }

        public override bool PawnHasCapacity(Pawn pawn)
        {
            return pawn.health.capacities.CapableOf(AultoLib.PawnCapacityDefOf.EMReceiving);
        }
    }
}
