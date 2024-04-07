using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class CommunicationReceiverWorker
    {
        public virtual bool PawnCanRecieveCommunication(Pawn pawn)
        {
            return false;
        }

        public virtual bool PawnHasCapacity(Pawn pawn)
        {
            return false;
        }
    }
}
