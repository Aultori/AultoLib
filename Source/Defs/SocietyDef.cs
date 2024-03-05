using RimWorld;
using System;
using System.Collections.Generic;
using Verse;


// TODO: a custom pawn generator to generate pawns for specific societies

namespace AultoLib
{
    /// <summary>
    /// Society definition and some utitilies.
    /// </summary>
    // I don't want anything in the SocietyDef to determine whether interactions
    // between other Societies will be negative or positive in a fixed way.
    // That should be dynamic, and be handled by something else because it's
    // related to individual thinking.
    public class SocietyDef : Def
    {
        /// <summary>
        /// The directory where <see cref="Database.TextFile_Loader"/> looks for files
        /// </summary>
        public string FolderPath => this.absolutePath ?? $"Socities/{this.folderName}/Strings";

        /// <summary>
        /// The fullyCapitalized keyword. <para/>
        /// The keyword that represents this society, used in <see cref="AultoLib.InteractionInstanceDef" />
        /// and in the datastructure of <see cref="AultoLib.Database.GrammarDatabase" />
        /// </summary>
        public string KeyUpper => this.keyUpper ?? (this.keyUpper = this.dataKeyword.ToUpper());
        /// <summary>
        /// The lowercase keyword
        /// </summary>
        public string KeyLower => this.keyLower ?? (this.keyLower = this.dataKeyword.ToUpper());

        /// <summary>
        /// keyword with it's first letter capitalized
        /// </summary>
        public string KeyCap => this.keyCap ?? (this.keyCap = this.dataKeyword.ToLower().CapitalizeFirst());


        /// <summary>
        /// Determines if <c>pawn</c> should behave like this culture.
        /// </summary>
        /// <param name="pawn">any pawn</param>
        /// <returns>
        /// Checks the <c>societyChecker</c> field in the Def if it exits,
        /// otherwise checks the <c>FleshTypeDef</c>s supplied.
        /// Defaults to <c>false</c>.
        /// </returns>
        public bool hasSociety(Pawn pawn)
        {
            if (this.societyChecker != null)
            {
                return this.societyChecker.Check(pawn);
            }
            if (this.fleshTypes.Contains(pawn.RaceProps.FleshType))
            {
                return true;
            }
            return false;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            // label is the culture's name
            if (this.label == null) yield return "label is null";

            if (this.dataKeyword == null) yield return "dataKeyword is null";

            if (this.globalUtility == null) yield return "globalUtility is null";

            if (this.fleshTypes == null && this.societyChecker == null) yield return "both fleshTypes and societyChecker are null";

            if (this.folderName == null && this.absolutePath == null) yield return "both folderName and absolutePath are null";

        }

        public override void ResolveReferences()
        {
            Database.SocietyDef_Loader.LoadToDatabase(this.KeyUpper, this);
            Database.SocietyDatabase.AddSociety(this);
            if (this.globalUtility?.ruleset == null)
            {
                Log.Error($"{Globals.LOG_HEADER} oops something asdjfhjakshdfahsldkjs... globalUtility doesn't exist");
            }
            else
            {
                // idk. this really should be executed somehow
                Database.Ruleset_Loader.LoadGlobalRuleset(this.globalUtility.ruleset, this.KeyUpper);
                Database.Ruleset_Loader.LoadGlobalRuleset(this.globalUtility.ruleset, this.KeyLower);
            }
            // DefDatabase<SocietyDef>.Add(this);
            Log.Message($"{Globals.DEBUG_LOG_HEADER} loaded the {this.defName} SocietyDef");
        }

        public static SocietyDef Named(string defName)
        {
            return DefDatabase<SocietyDef>.GetNamed(defName);
        }

        public static SocietyDef Get(string society_key)
        {
            return Database.SocietyDatabase.GetSocietyDef(society_key);
        }

        public override string ToString() => this.KeyUpper;


        // +------------------------+
        // |       The Data         |
        // +------------------------+

        /// <summary>
        /// The keyword that represents this culture, used in <see cref="AultoLib.InteractionInstanceDef" />
        /// and in the datastructure of <see cref="AultoLib.Database.GrammarDatabase" />
        /// </summary>
        private string dataKeyword;

        // // things for grammar
        // public string labelPlural;
        // public string definite;
        // public string indefinite;
        // public string possessive;

        public RulesetDef globalUtility;


        /// <summary>
        /// The race's <see cref="FleshTypeDef"/>s that can be used to identify a <see cref="Pawn"/>'s society
        /// </summary>
        public List<FleshTypeDef> fleshTypes; // make this default to normal flesh type
        /// <summary>
        /// useful for modders
        /// </summary>
        public SocietyChecker societyChecker;
        /// <summary>
        /// The name of the folder that <see cref="Database.TextFile_Loader"/> looks in. (case sensititve)
        /// </summary>
        private string folderName;
        /// <summary>
        /// Not required, but needed if the path is not the default one
        /// </summary>
        private string absolutePath;

        // restricting Pawns to communicate in certian languages would make interactions very complicated
        // public List<string> preferredLanguages;
        // public List<string> languages;

        /// <summary>
        /// communicationMethods.canInitate or communicationMethods.canRecieve:
        /// lists of preferred <see cref="CommunicationMethod"/> names, strings
        /// </summary>
        public CommunicationMethodLists communicationMethods;

        // +-------------+
        // |    TODO!    |
        // +-------------+
        // I think I'll need individual classes that desribe how Vilos or a specific race would try to interact.
        // they'd prefer interacting with people they like, and those nearby
        // they'd also prefer to use the communication methods they like, but their like for people has more weight on their decision
        // they also do things instinctually, like body language

        public List<string> preferredCommunicationMethods;

        public struct CommunicationMethodLists
        {
            /// <summary>
            /// preferred communicaions methods.
            /// one of these will be randomly chosen first, so they both happen equally
            /// </summary>
            // communication method categories
            public List<string> preferred;
            // these are related to the state of the pawn's body
            public List<string> canInitiate;
            public List<string> canRecieve;
        }

        /// <summary>
        /// if loaded into <see cref="AultoLib.Database.GrammarDatabase"/>
        /// </summary>
        [Unsaved(false)] internal bool loaded = false;
        [Unsaved(false)] private string keyUpper; // capital keyword
        [Unsaved(false)] private string keyLower; // lowercase keyword
        [Unsaved(false)] private string keyCap; // keyword with first letter capitalized
    }
}
