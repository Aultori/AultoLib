using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_Pawn_Society
    {
        /// <summary>
        /// I want to have pawn societies set at pawn generation
        /// <br/>but I'm testing so right now I'll use the race's society
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static SocietyDef Society(this Pawn pawn)
        {
            // if (pawn_Society.TryGetValue(pawn, out SocietyDef def)) return def;
            // def = AultoLib.SocietyDefOf.fallback;
            // if ()
            SocietyDef def = pawn.RaceProps.DefaultSociety();
            return def; // will sometimes equal null if the race doesn't have a default
        }

        public static bool HasSociety(this Pawn pawn) => pawn.Society() != null;

        public static bool CanHaveSociety(this Pawn pawn)
        {
            return pawn.RaceProps.intelligence >= Verse.Intelligence.ToolUser;
        }



        private static readonly CustomProperty<Pawn, SocietyDef> pawn_Society = new CustomProperty<Pawn, SocietyDef>();
    }
}
