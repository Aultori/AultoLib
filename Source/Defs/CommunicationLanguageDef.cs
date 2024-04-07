using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// The language a communication uses. usually "Common", the common language of RimWorld, but it can be something culture specific
    /// <br/>
    /// The defName is simply a keyword to keep track of which languages a society/race is able to use.
    /// <br/>
    /// This can be something like "sign language"
    /// </summary>
    public class CommunicationLanguageDef : Def
    {

        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;

            // if (this.defName != this.defName.ToLower()) yield return "defName not all lowercase"; // defName will be used as a key
        }

        public string CommunicationPrefix() => (alwaysShow || showWhenUsed && inUseWord != null) ? $"[{inUseWord}] " : null;

        /// <summary>
        /// The medium this language travels through
        /// </summary>
        public CommunicationMediumDef medium;
        // /// <summary>
        // /// Tags describing neurological language processing traits: The brain's ability to learn types of alien languages.
        // /// <br/> For instance, if a species is sensitive in the frequencies used in communication
        // /// </summary>
        // public List<string> requiredNeurologicalTraits;
        // /// <summary>
        // /// aaaa
        // /// </summary>
        // // do this later. for now assume all languages can be learned
        // public LanguageDifficulty neurologicalDifficulty;
        /// <summary>
        /// If all pawns know this language
        /// </summary>
        public bool knownByDefault = false;
        // /// <summary>
        // /// The name of the language;
        // /// </summary>
        // // use label instead
        // public string properNoun;
        /// <summary>
        /// How the language is denoted when a pawn is using it. (the part within the brackets)
        /// <br/>such as "[signing] Foo told Bar about ..."
        /// <br/>such as "[radio] Foo told Bar about ..."
        /// <br/>such as "[body language] Foo told Bar about ..."
        /// </summary>
        public string inUseWord;
        /// <summary>
        /// If true, then at the beginning of a pawn's interaction, denote the language in use
        /// </summary>
        public bool showWhenUsed = false;


        [Unsaved(false)]
        public static bool alwaysShow = true; // set to false when not debugging

        // // This is getting more complicated than I thought...
        // public enum LanguageDifficulty : byte
        // {
        //     SpeciesSpecific,
        //     Humanlike,
        //     Cyber
        // }
    }

}
