using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.Grammar
{
    /// <summary>
    /// words will be randomly selected here.
    /// I can still have multiple ExtendedRule_Word with the same keyword.
    /// </summary>
    public class ExtendedRule_Word : ExtendedRule
    {
        public override RuleSegments GetSegments()
        {
            return new RuleSegments(GetRandomWord());
        }

        public override void Initialize(string fileContents)
        {
            string[] groupSep = new string[] { "\r\n\r\n", "\n\n" };
            string[] lineSep  = new string[] { "\r\n", "\n" };
            string comment  = "//";

            this.wordData = new List<List<string>>(); // build from scratch

            // split each paragraph apart.
            string[] paragraphs = fileContents.Split(groupSep, StringSplitOptions.RemoveEmptyEntries);
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
                this.wordData.Add(words);
            }
        }

        /// <summary>
        /// chooses a random word from the group
        /// </summary>
        /// <returns>a word</returns>
        public string GetRandomWord()
        {
            List<string> paragraph = wordData[Rand.Range(0,wordData.Count)];
            return paragraph[Rand.Range(0,paragraph.Count)];
        }

        public List<List<string>> wordData;
    }
}
