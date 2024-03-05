using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.Grammar
{
    /// <summary>
    /// The thing that appears inside <c>[]</c> in a rule string
    /// </summary>
    public struct RuleMacro
    {

        public static RuleMacro NewEmpty()
        {
            return new RuleMacro {isMacro = false, key = null, society = null};
        }

        // there should be no leading or trailing spaces
        public RuleMacro (string text)
        {
            Regex testforLocalConstant = new Regex("^[A-Z]+(?:_[A-Za-z]+)?$"); // [INITIATOR_key]
            Regex testforSociety = new Regex("^(?<society>(?>[A-Z]+)(?=\\|))?\\|?(?<key>(?>[0-9A-Za-z_]+))$"); // [CULTURE|MacroName] [INITIATORCULTURE|MacroName]


            if (testforLocalConstant.Match(text).Success)
            {
                this.isMacro = false;
                this.key = text;
                this.society = ""; // idk if this sould be null
                return;
            }

            Match match = testforSociety.Match(text);
            if (match.Success)
            {
                this.isMacro = true;
                this.key = match.Groups["key"].Value;
                this.society = match.Groups["society"].Value;
                if (this.society.NullOrEmpty()) this.society = Globals.INSTANCE_SOCIETY_KEY;
                // this.society = (this.society != null) ? this.society : "ACTIVE"; // active society if not specified
                return;
            }

            Log.Error($"{Globals.LOG_HEADER} Malformed rule macro: [{text}]");
            this.isMacro = false;
            this.key = null;
            this.society = null;
        }


        public override string ToString()
        {
            if (!this.MadeSuccesfully) return "[ERROR: Invalid Macro]";
            return (society.NullOrEmpty()) ? $"[{key}]" : $"[{society}|{key}]" ;
        }

        public bool MadeSuccesfully => !(isMacro==false && key==null && society==null);

        /// <summary>
        /// If this macro should look for something in the local constants, otherwise it's a regular macro
        /// </summary>
        public bool isMacro;
        /// <summary>
        /// The key to expand to.
        /// Could be for a local constant or a rule from a society.
        /// </summary>
        public string key;
        /// <summary>
        /// The society to search in.
        /// </summary>
        public string society;
    }
}
