using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Grammar;
using AultoLib.Grammar.Macro;
using Verse;


namespace AultoLib.Database
{
    public static class WordFile_Loader
    {

        private static void LoadToDatabase(string PREFIX, string keyword, ExtendedRule_Word wordFile)
        {
            GrammarDatabase.loadedWordFiles[PREFIX][keyword] = wordFile;
#if DEBUG
            Log.Message($"{Globals.DEBUG_LOG_HEADER} Loaded WordFile: {PREFIX} {keyword}");
#endif
        }

        // using Rule_String because it has an output variable, which is the file path I need
        public static void Load(Rule_String fileRule, CultureDef culture)
        {
            // to grab the right datae from GrammarDatabase.loadedCulturalFiles, I need
            // culture prefix --> path --> returns raw text
            // to store somthing in GrammarDatabase.loadedWordFiles, I need
            // culture prefix --> keyword --> WordFile
            string prefix = culture.PREFIX;
            string path = fileRule.Generate();
            string rawText = GrammarDatabase.loadedCulturalFiles[prefix][path];
            ExtendedRule_Word wordFile = new ExtendedRule_Word(rawText);
            string keyword = fileRule.keyword;

            if (!GrammarDatabase.loadedWordFiles.ContainsKey(prefix))
            {
                // element for culture doesn't exist, create it
                GrammarDatabase.loadedWordFiles.Add(prefix, new Dictionary<string, ExtendedRule_Word>());
            }
            LoadToDatabase(prefix, keyword, wordFile);
        }
        
    }
}
