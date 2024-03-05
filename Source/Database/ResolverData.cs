using System;
using System.Collections.Generic;
using Verse;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    /// <summary>
    /// For storing data used by the resolver and other classes during the active process of building sentences.
    /// also data about resolver opperation.
    /// GrammarDatabase uses this class so the correct rules are looked up.
    /// </summary>
    public static class ResolverData
    {
        public static void Reset()
        {
            runID++;
            localConstants.Clear();
            extraTags.Clear();
            resolvedTags.Clear();
            outTags.Clear();
        }

        public static string GetConstant(string key)
        {
            return localConstants.Get(key);
        }


        // +-----------------------+
        // |    Flag-like Stuff    |
        // +-----------------------+


        /// <summary>
        /// The culture that will be looked in first when encountering a macro that doesn't have a specified culture.
        /// </summary>
        public static string activeCulture;


        public static bool isResolving; // whether the resolver is currently active and working on things.

        /// <summary>
        /// this will make it 
        /// </summary>
        public static Int64 runID = 0;


        // +-----------------+
        // |    Variables    |
        // +-----------------+

        /// <summary>
        /// Values relevant to the in game situation that the generated sentence will describe.
        /// Generated and assigned to before sentence resolution.
        /// </summary>
        public static Constants localConstants = new Constants();

        public static List<string> extraTags = new List<string>();
        public static List<string> resolvedTags = new List<string>();
        public static List<string> outTags = new List<string>();

        public static string debugLabel = null;
        public static bool forceLog = false;
        public static bool capitalizeFirstSentence = true;
    }
}
