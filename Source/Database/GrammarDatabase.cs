using System;
using System.Collections.Generic;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    public class GrammarDatabase
    {
        // +------------------------+
        // |     The Variables      |
        // +------------------------+

        // InteractionInstance data
        // category --> initiator culture --> recipient culture --> communication method --> InteractionInstance

        /// <summary>
        /// <b>InteractionInstance data:</b>
        /// category --> initiator culture --> recipient culture --> communication method --> InteractionInstance
        /// </summary>
        internal static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, InteractionInstanceDef>>>>
                loadedInteractionInstances = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, InteractionInstanceDef>>>>();

        /// <summary>
        /// <b>Rule data</b>
        /// </summary>
        private int rulesDatabase;

        // prefix --> path --> rawText
        internal static Dictionary<string, Dictionary<string, string>>
            loadedCulturalFiles = new Dictionary<string, Dictionary<string, string>>();

        // culture prefix --> keyword --> WordGroup
        internal static Dictionary<string, Dictionary<string, WordFile>>
            loadedWordFiles = new Dictionary<string, Dictionary<string, WordFile>>();

        internal static Dictionary<string,CultureDef>
            loadedCultureDefs = new Dictionary<string,CultureDef>();
    }
}
