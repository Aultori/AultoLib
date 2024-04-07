using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib
{
    public class CommunicationMediumWorker
    {
        public virtual bool CommunicationCanReach(Pawn initiator, Pawn recipient)
        {
            return false;
        }

        public virtual bool CommunicationCanReach(IntVec3 cell, IntVec3 recipientCell, Map map)
        {
            return false;
        }
    }
}
