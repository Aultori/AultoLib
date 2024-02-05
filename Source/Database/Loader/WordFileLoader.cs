using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Grammar;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    public static class WordFileLoader
    {

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
            WordFile wordFile = new WordFile(rawText);
            string keyword = fileRule.keyword;

            if (!GrammarDatabase.loadedWordFiles.ContainsKey(prefix))
            {
                // element doesn't exist, create it
                GrammarDatabase.loadedWordFiles.Add(prefix, new Dictionary<string, WordFile>());
            }
            GrammarDatabase.loadedWordFiles[prefix][keyword] = wordFile;
        }
        
    }
}
