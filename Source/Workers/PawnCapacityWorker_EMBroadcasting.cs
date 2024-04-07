using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    // note that this uses RimWorld's BodyPartTagDefOf.TalkingSource
    public class PawnCapacityWorker_EMBroadcasting : PawnCapacityWorker
    {
        public override float CalculateCapacityLevel(HediffSet diffSet, List<PawnCapacityUtility.CapacityImpactor> impactors = null)
        {
            return PawnCapacityUtility.CalculateTagEfficiency(diffSet, AultoLib.BodyPartTagDefOf.EMBroadcastingSource, impactors: impactors) * base.CalculateCapacityAndRecord(diffSet, RimWorld.PawnCapacityDefOf.Consciousness, impactors);
        }

        public override bool CanHaveCapacity(BodyDef body)
        {
            return body.HasPartWithTag(AultoLib.BodyPartTagDefOf.EMBroadcastingSource);
        }
    }
}
