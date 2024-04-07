using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_RaceProperties_Communications
    {
        public static RaceCommunications Communications(this RaceProperties raceProps)
        {
            if (race_Communications.TryGetValue(raceProps, out var def))
                return def;
            return null;
        }

        public static void LinkRaceCommunicationsDef(this RaceProperties raceProps, RaceCommunications def)
        {
            race_Communications.SetValue(raceProps, def);
        }

        private static readonly CustomProperty<RaceProperties, RaceCommunications> race_Communications = new CustomProperty<RaceProperties, RaceCommunications>();
    }
}
