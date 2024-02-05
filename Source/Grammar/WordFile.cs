using RimWorld;
using System;
using System.Collections.Generic;

namespace AultoLib.Grammar
{
    /// <summary>
    /// aids in randomly selecting a word from a list.
    /// words in smaller paragraphs are more likely to appear
    /// </summary>
    public class WordFile
    {
        /// <summary>
        /// builds the data
        /// </summary>
        /// <param name="fileData"></param>
        public WordFile(string fileData)
        {
            Generate(fileData);
        }

        private void Generate(string fileData)
        {
            string[] groupSep = new string[] { "\r\n\r\n", "\n\n" };
            string[] lineSep  = new string[] { "\r\n", "\n" };
            string comment  = "//";

            this.wordData = new List<List<string>>(); // build from scratch

            // split each paragraph apart.
            string[] paragraphs = fileData.Split(groupSep, StringSplitOptions.RemoveEmptyEntries);
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
        public string GetRandomWord(Random random)
        {
            List<string> paragraph = wordData[random.Next(wordData.Count)];
            return paragraph[random.Next(paragraph.Count)];
        }


        private List<List<string>> wordData;
    }

}
