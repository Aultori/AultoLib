using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib
{
    public interface CommunicationMethod
    {
        /// <summary>
        /// Gets the name of the communication method. for instance "talking", "bodylanguage", etc.
        /// </summary>
        string Name { get; }

        bool PawnCanInitiate(Pawn pawn, CultureDef culture = null);

        bool PawnCanRecieve(Pawn pawn, CultureDef culture = null);
    }
}
