using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class CommunicationMediumWorker_MagneticField : CommunicationMediumWorker
    {
        public override bool CommunicationCanReach(Pawn initiator, Pawn recipient)
        {
            return CommunicationCanReach(initiator.Position, recipient.Position, initiator.Map);
        }

        public override bool CommunicationCanReach(IntVec3 cell, IntVec3 recipientCell, Map map)
        {
            return GenSight.LineOfSight(cell, recipientCell, map);
        }
    }
}
