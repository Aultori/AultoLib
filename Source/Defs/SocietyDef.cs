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

        public SocietyChecker Checker
        {
            get
            {
                if (this.checkerInt == null)
                    this.checkerInt = (SocietyChecker)Activator.CreateInstance(this.checkerClass);
                return this.checkerInt;
            }
        }

        /// <summary>
        /// The directory where <see cref="Database.TextFile_Loader"/> looks for files
        /// </summary>
        public string FolderPath => this.absolutePath ?? $"Societies/{this.folderName}";

        /// <summary>
        /// The fullyCapitalized keyword. <para/>
        /// The keyword that represents this society, used in <see cref="AultoLib.InteractionInstanceDef" />
        /// and in the datastructure of <see cref="AultoLib.Database.GrammarDatabase" />
        /// </summary>
        // public string Key => this.keyUpper ?? (this.keyUpper = this.dataKeyword.ToUpper());
        public string Key => this.keyUpper ?? (this.keyUpper = this.defName.ToUpper());
        // /// <summary>
        // /// The lowercase keyword
        // /// </summary>
        // public string KeyLower => this.keyLower ?? (this.keyLower = this.dataKeyword.ToUpper());

        // /// <summary>
        // /// keyword with it's first letter capitalized
        // /// </summary>
        // public string KeyCap => this.keyCap ?? (this.keyCap = this.dataKeyword.ToLower().CapitalizeFirst());


        /// <summary>
        /// Determines if <c>pawn</c> should behave like this culture.
        /// </summary>
        /// <param name="pawn">any pawn</param>
        /// <returns>
        /// Checks the <c>societyChecker</c> field in the Def if it exits,
        /// otherwise checks the <c>FleshTypeDef</c>s supplied.
        /// Defaults to <c>false</c>.
        /// </returns>
        public bool HasSociety(Pawn pawn)
        {
            // if (!checkable) return false;
            if (this.checkerClass != typeof(SocietyChecker))
                return this.Checker.Check(pawn);
            if (this.fleshTypes.Contains(pawn.RaceProps.FleshType))
                return true;
            return false;
        }

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors())
            {
                yield return error;
            }

            if (this.defName != this.defName.ToLower()) yield return "defName not all lowercase. needed because it's used as a key";

            // label is the culture's name
            if (this.label == null) yield return "label is null";

            // if (this.dataKeyword == null) yield return "dataKeyword is null";

            if (this.globalUtility == null) yield return "globalUtility is null";

            //if (this.fleshTypes == null && this.checkerClass == null) yield return "both fleshTypes and societyChecker are null";
            if (this.fleshTypes == null) yield return "fleshTypes is null";

            if (this.folderName == null && this.absolutePath == null) yield return "both folderName and absolutePath are null";

        }

        public override void ResolveReferences()
        {
            if (AultoLog.DoLog()) AultoLog.Message($"loading {this.ColoredDefInformation()}");
            //Database.SocietyDef_Loader.LoadToDatabase(this.KeyUpper, this);
            Database.GrammarDatabase.loadedSocietyDefs[this.Key] = this;
            Database.SocietyDatabase.AddSociety(this);
            if (this.globalUtility?.ruleset == null)
            {
                // Log.Error($"{Globals.LOG_HEADER} oops something asdjfhjakshdfahsldkjs... globalUtility doesn't exist");
                AultoLog.Error("this shouldn't be possible. globalUtility's ruleset was null");
            }
            else
            {
                // idk. this really should be executed somehow
                Database.Ruleset_Loader.LoadGlobalRuleset(this.globalUtility.ruleset, this.Key);
            }
            // DefDatabase<SocietyDef>.Add(this);
            // Log.Message($"{Globals.DEBUG_LOG_HEADER} loaded the {this.defName} SocietyDef");
            if (AultoLog.DoLog())
            {
                // string thisDef = Logging.ColorText($"{nameof(AultoLib.SocietyDef)}: {this.defName}", "yellow");
                // Logging.Message($"loaded {thisDef} to the database");
                AultoLog.Message($"loaded {this.ColoredDefInformation()} to the database");
            }
        }

        public static SocietyDef Named(string defName)
        {
            try
            { return DefDatabase<SocietyDef>.GetNamed(defName.ToLower()); }
            catch
            { return SocietyDefOf.fallback; }
        }

        //public static SocietyDef Get(string society_key)
        //{
        //    return Database.SocietyDatabase.GetSocietyDef(society_key);
        //}

        public override string ToString() => this.Key;


        // +------------------------+
        // |       The Data         |
        // +------------------------+

        // /// <summary>
        // /// The keyword that represents this culture, used in <see cref="AultoLib.InteractionInstanceDef" />
        // /// and in the datastructure of <see cref="AultoLib.Database.GrammarDatabase" />
        // /// </summary>
        //protected string dataKeyword;

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
        protected Type checkerClass = typeof(SocietyChecker); // idk how to get this to work
        /// <summary>
        /// The name of the folder that <see cref="Database.TextFile_Loader"/> looks in. (case sensititve)
        /// </summary>
        protected string folderName;
        /// <summary>
        /// Not required, but needed if the path is not the default one
        /// </summary>
        protected string absolutePath;

        // restricting Pawns to communicate in certian languages would make interactions very complicated
        // public List<string> preferredLanguages;
        // public List<string> languages;

        // /// <summary>
        // /// communicationMethods.canInitate or communicationMethods.canRecieve:
        // /// lists of preferred <see cref="CommunicationMethod"/> names, strings
        // /// </summary>
        // public CommunicationMethodLists communicationMethods;

        // +-------------+
        // |    TODO!    |
        // +-------------+
        // I think I'll need individual classes that desribe how Vilos or a specific race would try to interact.
        // they'd prefer interacting with people they like, and those nearby
        // they'd also prefer to use the communication methods they like, but their like for people has more weight on their decision
        // they also do things instinctually, like body language


        public List<string> preferredCommunicationMethods;

        /// <summary>
        /// A list of languages in order from most preferred.
        /// </summary>
        public List<CommunicationLanguageDef> learnedLanguages;

        // public struct CommunicationMethodLists
        // {
        //     /// <summary>
        //     /// preferred communicaions methods.
        //     /// one of these will be randomly chosen first, so they both happen equally
        //     /// </summary>
        //     // communication method categories
        //     public List<string> preferred;
        //     // these are related to the state of the pawn's body
        //     public List<string> canInitiate;
        //     public List<string> canRecieve;
        // }

        /// <summary>
        /// if loaded into <see cref="AultoLib.Database.GrammarDatabase"/>
        /// </summary>
        [Unsaved(false)] internal bool loaded = false;
        [Unsaved(false)] private string keyUpper; // capital keyword
        // [Unsaved(false)] private string keyLower; // lowercase keyword
        // [Unsaved(false)] private string keyCap; // keyword with first letter capitalized
        [Unsaved(false)] private SocietyChecker checkerInt;
    }
}
