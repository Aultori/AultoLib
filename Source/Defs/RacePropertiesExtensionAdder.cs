using System;
using Verse;
using RimWorld;
using System.Collections.Generic;

namespace AultoLib
{
    /// <summary>
    /// This will sort of add cusom properties to a pawn's race. <br/>
    /// The defName could be anything, it doesn't matter here.
    /// </summary>
    public class RacePropertiesExtensionAdder : Def
    {
        public override void PostLoad()
        {
            communicationCapacites.PostLoad();
        }

        public override void ResolveReferences()
        {
            RacePropsCommunicationCapacities.AddProperty(modifies.race, communicationCapacites);
        }

        /// <summary>
        /// The Pawn Race this effects
        /// </summary>
        public ThingDef modifies;

        // List<CommunicationMethod> communicationMethods;

        public CommunicationCapacities communicationCapacites;
    }


    public static class RacePropsCommunicationCapacities
    {
        public static CommunicationCapacities GetCommunicationCapacities(this RaceProperties props)
        {
            if (customProperties.TryGetValue(props, out var communicationCapacities))
                return communicationCapacities;
            return null;
        }

        public static void AddProperty(RaceProperties props, CommunicationCapacities capacities)
        {
            customProperties.SetValue(props, capacities);
        }

        private static readonly CustomProperties<RaceProperties, CommunicationCapacities> customProperties = new CustomProperties<RaceProperties, CommunicationCapacities>();
    }
}
