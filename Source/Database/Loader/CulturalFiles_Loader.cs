using System;
using System.Collections.Generic;
using System.IO;
using RimWorld.IO;
using Verse;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    /// <summary>
    /// This class loads data from the culture's file directory into GrammarDatabase.
    /// This class makes a CulturalRuleFile object from rule files in mod directories, then loads that into <c>Database.GrammarDatabase</c>
    /// </summary>
    public static class CulturalFiles_Loader
    {
        public static bool DataIsLoaded(CultureDef culture)
        { return Database.GrammarDatabase.loadedCulturalFiles.ContainsKey(culture.PREFIX); }

        public static bool DataIsLoaded(string PREFIX)
        { return Database.GrammarDatabase.loadedCulturalFiles.ContainsKey(PREFIX); }

        private static void LoadToDatabase(string PREFIX, string path, string fileText)
        {
            GrammarDatabase.loadedCulturalFiles[PREFIX].Add(path, fileText);
#if DEBUG
            Log.Message($"{Globals.DEBUG_LOG_HEADER} Loaded file {PREFIX} {path}");
#endif
        }

        /// <summary>
        /// Get a specific directory, looking in all mod folders.
        /// starts at the root of RimWorld file system.
        /// You can search for things in these directories.
        /// </summary>
        /// <param name="directory">Path to desired directory relative to the root of the RimWorld file system</param>
        /// <returns>an enumerator of directories</returns>
        // Tuple<VirtualDirectory, ModContentPack, string>
        public static IEnumerable<SomeModDirectory> GetDirectories(string directory)
        {
            foreach (ModContentPack mod in LoadedModManager.RunningMods)
            {
                foreach (string text in mod.foldersToLoadDescendingOrder)
                {
                    string path = Path.Combine(text, directory);
                    VirtualDirectory dir = AbstractFilesystem.GetDirectory(path);
                    if (dir.Exists)
                    {
                        yield return new SomeModDirectory { dir = dir, mod = mod, folder = text };
                    }
                }
            }
            yield break;
        }

        // currently only loads "Strings/..."
        public static void Load(CultureDef culture)
        {
            if (DataIsLoaded(culture)) return;
            // data isn't loaded, so
            // path --> fileData
            Dictionary<string, string> fileStructure = new Dictionary<string, string>();
            Database.GrammarDatabase.loadedCulturalFiles.Add(culture.PREFIX, fileStructure);
            //
            DeepProfiler.Start($"{Globals.LOG_HEADER} Loading culture data: {culture.defName} ({culture.Name})");
            try
            {
                CulturalFiles_Loader.tmpAlreadyLoadedFiles.Clear();
                
                foreach (SomeModDirectory modDir in GetDirectories(culture.MainPath))
                {
                    SomeModDirectory localDir = modDir;
                    if (!CulturalFiles_Loader.tmpAlreadyLoadedFiles.ContainsKey(localDir.mod))
                    {
                        // create new element if it doesn't exist already
                        CulturalFiles_Loader.tmpAlreadyLoadedFiles[localDir.mod] = new HashSet<string>();
                    }

                    VirtualDirectory dir = localDir.dir.GetDirectory("Strings");
                    if (dir.Exists)
                    {
                        foreach (VirtualDirectory virtualDir in dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
                        foreach (VirtualFile virtualFile in virtualDir.GetFiles("*.txt", SearchOption.AllDirectories))
                        {
                            if (TryRegisterFileIfNew(localDir, virtualFile.FullPath, culture))
                            {
                                LoadFile(virtualFile, dir, culture);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Error($"{Globals.LOG_HEADER} Exception loading culture data. Exception: {e}");
            }
            finally
            {
                DeepProfiler.End();
            }

        }

        private static bool TryRegisterFileIfNew(SomeModDirectory modDir, string filePath, CultureDef culture)
        {
#if DEBUG
            // I might not need this
            if (!DataIsLoaded(culture))
            {
                Log.Error($"{Globals.DEBUG_LOG_HEADER} Error executing TryRegisterFileIfNew(...): the key for CultureDef {culture.defName} doesn't exist in GrammarDatabase. It should already exist! something went wrong");
                return false;
            }
#endif

            if (!filePath.StartsWith(modDir.folder))
            {
                Log.Error($"{Globals.LOG_HEADER} Failed to get a relative path for a file of culture {culture.defName}: {filePath}, located in {modDir.folder}");
            }
            string item = filePath.Substring(modDir.folder.Length);
            if (!tmpAlreadyLoadedFiles.ContainsKey(modDir.mod))
            {
                tmpAlreadyLoadedFiles[modDir.mod] = new HashSet<string>();
            }
            else if (tmpAlreadyLoadedFiles[modDir.mod].Contains(item))
            {
                return false;
            }
            tmpAlreadyLoadedFiles[modDir.mod].Add(item);
            return true;
        }

        private static void LoadFile(VirtualFile file, VirtualDirectory stringsTopDir, CultureDef culture)
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
            // idk what this does but it was in RimWorld
            text2 = text2.Substring(0, text2.Length - Path.GetExtension(text2).Length);
            text2 = text2.Replace('\\', '/');
            // end idk
            LoadToDatabase(culture.PREFIX, file.ToString(), text2);

        }


        // +------------------------+
        // |     The Variables      |
        // +------------------------+

        // To keep track of files I loaded so I know to skip them.
        private static Dictionary<ModContentPack, HashSet<string>>
            tmpAlreadyLoadedFiles = new Dictionary<ModContentPack, HashSet<string>>();

        public readonly static string fallbackPath = "Languages/English";

        public struct SomeModDirectory
        {
            public VirtualDirectory dir;
            public ModContentPack mod;
            public string folder;
        }
    }
}
