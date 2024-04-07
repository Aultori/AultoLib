using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI.Group;

namespace AultoLib
{
    public class CommunicationTransmitterWorker
    {
        public virtual bool PawnCanTransmitCommunication(Pawn pawn)
        {
            return false;
        }

        public virtual bool PawnHasCapacity(Pawn pawn)
        {
            return false;
        }
    }
}
