using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_RaceProperties_Languages
    {
        /// <summary>
        /// Innate languages of a race.
        /// The most preferred languages come first in the list
        /// </summary>
        /// <param name="raceProps"></param>
        /// <returns></returns>
        public static List<CommunicationLanguageDef> Languages(this RaceProperties raceProps)
        {
            if (race_Languages.TryGetValue(raceProps, out var languages))
                return languages;
            return null;
        }

        public static void LinkLanguages(this RaceProperties raceProps, List<CommunicationLanguageDef> languageList)
        {
            race_Languages.SetValue(raceProps, languageList);
        }

        private static readonly CustomProperty<RaceProperties, List<CommunicationLanguageDef>> race_Languages = new CustomProperty<RaceProperties, List<CommunicationLanguageDef>>();
    }
}
