using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// Default CommunicationMethod
    /// For now, it's the only one I'll implement so I can test things out, but I should create it with the idea that others will be added later
    /// </summary>
    public class CommunicationWorker_Talking : CommunicationWorker
    {
        public string Name => "talking";

        public override bool PawnCanInitiate(Pawn pawn)
        {
            return pawn.health.capacities.CapableOf(RimWorld.PawnCapacityDefOf.Talking)
                && pawn.Awake()
                && !pawn.IsBurning();
        }

        /// <summary>
        /// Can hear spoken language
        /// </summary>
        public override bool PawnCanRecieve(Pawn pawn)
        {
            return pawn.Awake() && !pawn.IsBurning();
        }

        public override bool PawnCanInitiateRandom(Pawn p)
        {
            return this.PawnCanInitiate(p);
        }

    }
}
