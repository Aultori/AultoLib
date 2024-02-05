using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using AultoLib.Culture;

namespace AultoLib
{
    /// <summary>
    /// Culture definition and some utitilies
    /// </summary>
    public class CultureDef : Def
    {
        /// <summary>The culture's name.</summary>
        public string Name
        { get { return this.name; } }

        /// <summary>The culture's name capitalized.</summary>
        public string NameCap
        { get { return this.nameCap; } }

        /// <summary>
        /// The prefix to use in
        /// <see cref="AultoLib.Grammar.RuleExtended" />
        /// </summary>
        public string PREFIX
        { get { return this.prefix; } }


        /// <summary>
        /// The directory where <c>Strings/</c>&lt;CulturalRuleFile>s are located
        /// </summary>
        public string MainPath
        {
            get 
            { 
                if (absolutePath != null)
                {
                    return absolutePath;
                }
                return String.Concat("Cultures/",label.CapitalizeFirst(),"/"); 
            }
        }

        /// <summary>
        /// Determines if <c>pawn</c> should behave like this culture.
        /// </summary>
        /// <param name="pawn">any pawn</param>
        /// <returns>
        /// Checks the <c>cultureChecker</c> field in the Def if it exits,
        /// otherwise checks the <c>FleshTypeDef</c>s supplied.
        /// Defaults to <c>false</c>.
        /// </returns>
        public bool HasCulture(Pawn pawn)
        {
            if (this.cultureChecker != null)
            {
                return this.cultureChecker.Check(pawn);
            }
            foreach (FleshTypeDef fleshType in this.fleshTypes)
            {
                if (fleshType == pawn.RaceProps.FleshType)
                    return true;
            }
            return false;
        }

        // +------------------------+
        // |       The Data         |
        // +------------------------+

        /// <summary>
        /// The culture's name.
        /// </summary>
        private string name;

        /// <summary>
        /// The culture's name capitalized.
        /// </summary>
        private string nameCap;

        /// <summary>
        /// The prefix used in <c>Rule</c>s
        /// </summary>
        private string prefix;

        public List<FleshTypeDef> fleshTypes; // make this default to normal flesh type

        /// <summary>
        /// useful for modders
        /// </summary>
        private CultureChecker cultureChecker = null;

        /// <summary>
        /// Not required, but needed if the path is not the default one
        /// </summary>
        private string absolutePath = null;


        /// <summary>
        /// communicationMethods.canInitate or communicationMethods.canRecieve:
        /// lists of preferred CommunicationMethod names, strings
        /// </summary>
        public CommunicationMethodLists communitaionMethods;
        
        public struct CommunicationMethodLists
        {
            /// <summary>
            /// preferred communicaions methods.
            /// one of these will be randomly chosen first, so they both happen equally
            /// </summary>
            public List<string> preferred;

            public List<string> canInitiate;

            public List<string> canRecieve;
        }
    }
}
