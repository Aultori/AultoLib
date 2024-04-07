using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public abstract class CommunicationWorker
    {
        public abstract bool PawnCanInitiate(Pawn pawn);

        /// <summary>
        /// Can hear spoken language
        /// </summary>
        public abstract bool PawnCanRecieve(Pawn pawn);

        public abstract bool PawnCanInitiateRandom(Pawn p);
    }
}
