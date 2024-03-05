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
    public class CommunicationMethod_Talking : CommunicationMethod
    {
        public string Name { get { return "talking"; } }

        public bool PawnCanInitiate(Pawn pawn, SocietyDef culture = null)
        {
            return pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking)
                && pawn.Awake()
                && !pawn.IsBurning();
        }

        /// <summary>
        /// Can hear spoken language
        /// </summary>
        public bool PawnCanRecieve(Pawn pawn, SocietyDef culture = null)
        {
            return pawn.Awake() && !pawn.IsBurning();
        }

        public bool PawnCanInitiateRandom(Pawn p)
        {
            return this.PawnCanInitiate(p, null);
        }

    }
}
