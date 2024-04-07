using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_RaceProperties_Society
    {
        /// <summary>
        /// The default society of a race. some races don't have a society.
        /// I want this to be used at pawn generation to assign societies to pawns
        /// but I can do that later
        /// </summary>
        /// <param name="raceProps"></param>
        /// <returns></returns>
        public static SocietyDef DefaultSociety(this RaceProperties raceProps)
        {
            if (race_DefaultSociety.TryGetValue(raceProps, out SocietyDef def)) return def;
            return null;
        }

        public static void LinkDefaultSociety(this RaceProperties raceProps, SocietyDef societyDef) => race_DefaultSociety.SetValue(raceProps, societyDef);

        private static readonly CustomProperty<RaceProperties, SocietyDef> race_DefaultSociety = new CustomProperty<RaceProperties, SocietyDef>();
    }
}
