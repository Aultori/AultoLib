using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_PawnAndSocietyAndRace_Languages
    {
        public static List<CommunicationLanguageDef> Learnedanguages(this Pawn pawn)
        {
            if (!pawn_PreferredLanguages.TryGetValue(pawn, out var preferredLanguages)) return null;
            return preferredLanguages;
        }
        public static List<CommunicationLanguageDef> KnownLanguages(this Pawn pawn)
        {
            var knownLanguages = new List<CommunicationLanguageDef>();
            var preferredLanguages = pawn.Learnedanguages();
            var innateLanguages = pawn.RaceProps.InnateLanguages();
            if (preferredLanguages != null) knownLanguages.AddRange(preferredLanguages);
            if (innateLanguages != null) knownLanguages.AddRange(innateLanguages);

            return knownLanguages;
        }

        public static List<CommunicationLanguageDef> InnateLanguages(this RaceProperties race)
        {
            if (!race_InnateLanguages.TryGetValue(race, out var innateLanguages)) return null;
            return innateLanguages;
        }

        // public static List<CommunicationLanguageDef> PreferredLanguages(this SocietyDef society)
        // {
        //     return society.preferredLanguages;
        // }


        private static CustomProperty<Pawn, List<CommunicationLanguageDef>> pawn_PreferredLanguages = new CustomProperty<Pawn, List<CommunicationLanguageDef>>();
        private static CustomProperty<RaceProperties, List<CommunicationLanguageDef>> race_InnateLanguages = new CustomProperty<RaceProperties, List<CommunicationLanguageDef>>();
    }
}
