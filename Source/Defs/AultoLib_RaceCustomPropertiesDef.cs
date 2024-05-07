using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AultoLib.CustomProperties;

namespace AultoLib
{
    // TODO:
    // include stuff like communication frequency here
    // also things for how often body language is used

    public class AultoLib_RaceCustomPropertiesDef : Def
    {
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;
            if (thingDef == null) yield return $"{nameof(thingDef)} is null";

            if (communications != null)
                foreach (string error in communications.ConfigErrors()) yield return error;
        }

        public override void ResolveReferences()
        {
            communications.ResolveReferences();
            if (communications != null) thingDef.race.LinkRaceCommunicationsDef(communications);
            if (languageLearningAge != null) thingDef.race.LinkLanguageLearningAge((float) languageLearningAge);
            // if (languages != null) thingDef.race.LinkLanguages(languages);
            if (society != null) thingDef.race.LinkDefaultSociety(society);
        }


        public ThingDef thingDef;

        public SocietyDef society;

        // public List<CommunicationLanguageDef> languages;

        public float? languageLearningAge;

        public RaceCommunications communications;
    }
}
