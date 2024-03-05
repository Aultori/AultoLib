using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AultoLib.Society
{
    /// <summary>
    /// Society varient of RimWorld.Pawn_InteractionsTracker
    /// </summary>
    public class Pawn_InteractionsTracker : RimWorld.Pawn_InteractionsTracker, IExposable
    {
        public Pawn_InteractionsTracker(Pawn pawn) : base(pawn)
        {
        }
    }
}
