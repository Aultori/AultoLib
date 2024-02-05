using System;
using System.Collections.Generic;
using System.IO;
using RimWorld.IO;
using Verse;

namespace AultoLib.Culture
{
    /// <summary>
    ///
    /// The cultures of all mods.
    /// Very similar to RimWorld.LoadedLanguage.
    /// Note: currently only english is supported.
    ///
    /// most if not all aspects of working with the data structure are handled automatically by methods in this class.
    /// you really only need to be calling TryGetGroupsFromFile(...). It will look for and load the file if it wasn't found.
    /// </summary>
    public class LoadedCultures
    {

        /// <summary>
        /// Get a specific directory.
        /// starts at the root of RimWorld file system
        /// </summary>
        /// <param name="directory">The starting directory of a culture</param>
        /// <returns>an enumerator of directories</returns>
        public IEnumerable<Tuple<VirtualDirectory, ModContentPack, string>> GetDirectories(string directory)
        {
            foreach (ModContentPack mod in LoadedModManager.RunningMods)
            {
                foreach (string text in mod.foldersToLoadDescendingOrder)
                {
                    string path = Path.Combine(text, directory);
                    VirtualDirectory dir = AbstractFilesystem.GetDirectory(path);
                    if (dir.Exists)
                    {
                        yield return new Tuple<VirtualDirectory, ModContentPack, string>(dir, mod, text);
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Get the group datastructure.
        /// 
        /// </summary>
        /// <param name="fileName">self explanitory</param>
        /// <param name="culture">self explanitory</param>
        /// <param name="wordGroup">self explanitory</param>
        /// <returns>whether the method executed successfully</returns>
        public bool TryGetWordGroupFromFile(string fileName, CultureDef culture, out WordGroup wordGroup)
        {
            if (!DataIsLoaded(culture))
            {
                LoadCultureData(culture);
            }
            if (wordgroupFiles[culture.defName].TryGetValue(fileName, out wordGroup))
            {
                return true;
            }
            // check the fallback culture
            if (wordgroupFiles[CultureDefOf.fallback.defName].TryGetValue(fileName, out wordGroup))
            {
                return true;
            }
            wordGroup = null;
            return false;
        }

        /// <summary>
        /// similar to LoadedLanguage.LoadData with some changes
        /// </summary>
        public void LoadCultureData(CultureDef culture)
        {
            // the same as: if (!this.wordgroupFiles.ContainsKey(culture.defName))
            if (DataIsLoaded(culture))
            {
                return;
            }
            // create new element because it doesn't exist
            wordgroupFiles[culture.defName] = new Dictionary<string, WordGroup>();
            DeepProfiler.Start($"{Globals.LOG_HEADER} Loading culture data: {culture.defName} ({culture.label})");
            try
            {
                tmpAlreadyLoadedFiles.Clear();

                foreach (Tuple<VirtualDirectory, ModContentPack, string> tuple in GetDirectories(culture.MainPath))
                {
                    Tuple<VirtualDirectory, ModContentPack, string> localDir = tuple;
                    if (!tmpAlreadyLoadedFiles.ContainsKey(localDir.Item2))
                    {
                        // create new element if it doesn't exist
                        tmpAlreadyLoadedFiles[localDir.Item2] = new HashSet<string>();
                    }

                    VirtualDirectory dir = localDir.Item1.GetDirectory("Strings");
                    if (dir.Exists)
                    {
                        foreach (VirtualDirectory virtualDir in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
                        foreach (VirtualFile virtualFile in virtualDir.GetFiles("*.txt", SearchOption.AllDirectories))
                        {
                            if (TryRegisterFileIfNew(localDir, virtualFile.FullPath, culture))
                            {
                                LoadFromFile_Strings(virtualFile, dir, culture);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error($"{Globals.LOG_HEADER} Exception loading culture data. Exception: {ex}");
            }
            finally
            {
                DeepProfiler.End();
            }
        }

        /// <summary>
        /// 
        /// the same as LoadedLanguage.TryRegisterFileIfNew, but I needed to access this.tmpAlreadyLoadedFiles
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="filePath"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public bool TryRegisterFileIfNew(Tuple<VirtualDirectory, ModContentPack, string> dir, string filePath, CultureDef culture)
        {
#if DEBUG
            if (!DataIsLoaded(culture))
            {
                Log.Error($"{Globals.DEBUG_LOG_HEADER} Error executing TryRegisterFileIfNew(...): the key for CultureDef {culture.defName} doesn't exist in wordgroupFiles. It should already exist! something went wrong");
                return false;
            }
#endif

            if (!filePath.StartsWith(dir.Item3))
            {
                Log.Error($"{Globals.LOG_HEADER} Failed to get a relative path for a file of culture {culture.defName}: {filePath}, located in {dir.Item3}");
            }
            string item = filePath.Substring(dir.Item3.Length);
            if (!tmpAlreadyLoadedFiles.ContainsKey(dir.Item2))
            {
                tmpAlreadyLoadedFiles[dir.Item2] = new HashSet<string>();
            }
            else if (tmpAlreadyLoadedFiles[dir.Item2].Contains(item))
            {
                return false;
            }
            tmpAlreadyLoadedFiles[dir.Item2].Add(item);
            return true;
        }

        /// <summary>
        /// Again, basically the same as it is in LoadedLanguage.
        /// loads data from the files containing strings
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stringsTopDir"></param>
        /// <param name="culture"></param>
        private void LoadFromFile_Strings(VirtualFile file, VirtualDirectory stringsTopDir, CultureDef culture)
        {
            string text;
            try
            {
                text = file.ReadAllText();
            }
            catch (Exception ex)
            {
#if DEBUG
                Log.Error($"{Globals.DEBUG_LOG_HEADER} Exception from loading strings file {file}: {ex}");
#endif
                return;
            }
            string text2 = file.FullPath;
            if (stringsTopDir != null)
            {
                text2 = text2.Substring(stringsTopDir.FullPath.Length + 1);
            }
            text2 = text2.Substring(0, text2.Length - Path.GetExtension(text2).Length);
            text2 = text2.Replace('\\', '/');
            // uses my group data structure
            List<List<string>> groups = new List<List<string>>();
            foreach (List<string> item in GroupedLinesFromString(text2))
            {
                groups.Add(item);
            }
            wordgroupFiles[culture.defName].Add(text2, groups);
        }


        /// <summary>
        /// Groups lines together so words in smaller groups are more likely to appear.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>word groups</returns>
        public IEnumerable<List<string>> GroupedLinesFromString(string text)
        {
            string[] groupSep = new string[] { "\r\n\r\n", "\n\n" };
            string[] lineSep = new string[] { "\r\n", "\n" };
            string[] comment = new string[] { "//" };

            string[] group = text.Split(groupSep, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < group.Length; i++)
            {
                string[] lines = group[i].Trim().Split(lineSep, StringSplitOptions.RemoveEmptyEntries);
                List<string> list = new List<string>();
                for (int j = 0; j < lines.Length; j++)
                {
                    string text2 = lines[j].Trim();
                    if (!text2.StartsWith("//")) // remove comment from line
                    {
                        text2 = text2.Split(comment, StringSplitOptions.None)[0]; // remove comment from end of line
                        if (text2.Length != 0)
                        {
                            list.Add(text2);
                        }
                    }
                }
                if (list.Count > 0)
                {
                    yield return list;
                }
            }

            yield break;
        }

        /// <summary>
        /// Checks if the given culture if loaded.
        /// Explanation:
        /// The culture has loaded if the correct key is present in wordgroupFiles.
        /// The key is supposed to be the <c>defName</c> of the culture.
        /// However, this class gets the actual data from paths generated by the CultureDef's <c>absolutePath</c> or <c>LabelCap</c>,
        /// so try not to get confused.
        /// </summary>
        /// <param name="culture">the given culture</param>
        /// <returns>if the key exists</returns>
        private bool DataIsLoaded(CultureDef culture)
        {
            return wordgroupFiles.ContainsKey(culture.defName);
        }

        // I could make this look nicer later. maybe with a struct?
        // public Dictionary< string defName, Dictionary<string path, WordGroup groups >>
        private Dictionary<string, Dictionary<string, WordGroup>>
            wordgroupFiles = new Dictionary<string, Dictionary<string, WordGroup>>();

        private Dictionary<ModContentPack, HashSet<string>>
            tmpAlreadyLoadedFiles = new Dictionary<ModContentPack, HashSet<string>>();


        public readonly string fallbackPath = "Languages/English";

    }
}
