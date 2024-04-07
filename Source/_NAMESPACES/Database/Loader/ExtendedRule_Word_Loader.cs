using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AultoLib.Database;
using Verse;

namespace AultoLib.Grammar
{
    public class ExtendedRule_Word_Loader
    {
        public static Ruleset ToRuleset(List<string> rulesFiles, string CULTURE_KEY)
        {
            Ruleset pack = new Ruleset();

            foreach (string rawString in rulesFiles)
            {
                if (TryBuildRule(rawString, CULTURE_KEY, out ExtendedRule_Word rule))
                    pack.Add(rule);
            }

            return pack;
        }


        // I think this could be simplified
        public static bool TryBuildRule(string rawString, string societyKey, out ExtendedRule_Word wordRule)
        {
            ExtendedRule rule = new ExtendedRule_Word();
            wordRule = new ExtendedRule_Word();

            if (   ExtendedRule_Loader.TryBuildRule(rawString, rule, out string path)
                && TextFile_Loader.TryGetContents(SocietyDef.Named(societyKey), path, out string fileContents)
                && ExtendedRule_Word_Loader.TryBuildWords(fileContents, out List<List<string>> wordData) )
            {
                wordRule.BecomeCopyOf(rule);
                wordRule.wordData = wordData;
                return true;
            }

            wordRule = null;
            return false;
        }

        public static bool TryBuildWords(string fileContents, out List<List<string>> wordData)
        {
            string[] groupSep = new string[] { "\r\n\r\n", "\n\n" };
            string[] lineSep  = new string[] { "\r\n", "\n" };
            string comment  = "//";

            wordData = new List<List<string>>(); // build from scratch

            // split each paragraph apart.
            string[] paragraphs = fileContents.Split(groupSep, StringSplitOptions.RemoveEmptyEntries);

            // I could add more statements like this.
            if (paragraphs.Length == 0)
            {
                Log.Error($"{Globals.LOG_HEADER} no paragraphs in file.");
                return false;
            }

            // process each paragraph
            for (int i = 0; i < paragraphs.Length; i++)
            {
                string[] lines = paragraphs[i].Trim().Split(lineSep, StringSplitOptions.RemoveEmptyEntries);
                List<string> words = new List<string>();
                // process each line
                for (int j = 0; j < lines.Length; j++)
                {
                    string line = lines[j];
                    // skip if comment
                    if (!line.StartsWith(comment))
                    {
                        // remove comment from end
                        line = line.Split(new string[] {comment}, StringSplitOptions.None)[0];
                        // add if not empty
                        if (line.Length > 0 ) words.Add(line);
                    }
                }
                wordData.Add(words);
            }
            return true;
        }
    }
}
