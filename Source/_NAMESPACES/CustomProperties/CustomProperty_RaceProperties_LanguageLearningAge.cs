using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.CustomProperties
{
    public static class CustomProperty_RaceProperties_LanguageLearningAge
    {
        public static float LanguageLearningAge(this RaceProperties raceProps)
        {
            if (race_LanguageLearningAge.TryGetValue(raceProps, out var languageLearningAge))
                return languageLearningAge;
            return float.MaxValue;
        }

        public static void LinkLanguageLearningAge(this RaceProperties raceProps, float languageLearningAge)
        {
            race_LanguageLearningAge.SetValue(raceProps, languageLearningAge);
        }

        private static readonly CustomProperty<RaceProperties, float> race_LanguageLearningAge = new CustomProperty<RaceProperties, float>();
    }
}
