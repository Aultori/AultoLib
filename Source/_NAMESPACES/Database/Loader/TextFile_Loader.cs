using System;
using System.Collections.Generic;
using System.IO;
using RimWorld.IO;
using Verse;
using AultoLib.Grammar;
using UnityEngine;

namespace AultoLib.Database
{

    public static class TextFile_Loader
    {
        public static bool TryGetContents(SocietyDef societyDef, string path, out string rawText)
        {
            // AultoLog.Message($"Attempting to load {societyDef.FolderPath}/{path}");

            if (path.NullOrEmpty())
            {
                // AultoLog.Error($"TextFile_Loader.TryGetContents(...)  path is null");
                AultoLog.Error("TryGetContents() path is null");
                rawText = null;
                return false;
            }


            if (!DataIsLoaded(societyDef.Key))
            {
                TextFile_Loader.Load(societyDef);
            }
            if (GrammarDatabase.loadedTextFiles[societyDef.Key]?.TryGetValue(path, out rawText) == true)
            {
                // AultoLog.Message($"loaded {rawText}");
                return true;
            }

            if (AultoLog.DoLog()) AultoLog.Message($"file {path} not found in {societyDef.ColoredDefInformation()}");
            // look for it in the fallback society
            if (!DataIsLoaded(SocietyDefOf.fallback.Key))
                TextFile_Loader.Load(SocietyDefOf.fallback);
            if (GrammarDatabase.loadedTextFiles[SocietyDefOf.fallback.Key]?.TryGetValue(path, out rawText) == true)
                return true;

            AultoLog.Error($"could not find {societyDef.FolderPath}/{path}");
            rawText = null;
            return false;
        }

        private static bool DataIsLoaded(string society_key) => GrammarDatabase.loadedTextFiles.ContainsKey(society_key);


        private static void LoadToDatabase(string society_key, string path, string fileText)
        {
            GrammarDatabase.loadedTextFiles[society_key].Add(path, fileText);
            if (AultoLog.DoLog("file loading")) AultoLog.Message($"Loaded file {society_key} {path}");
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
            // AultoLog.Message($"looking for: {directory}");
            foreach (ModContentPack mod in LoadedModManager.RunningMods)
            {
                // AultoLog.Message($"Getting Mod Directories {mod.FolderName}");
                foreach (string text in mod.foldersToLoadDescendingOrder)
                {
                    //AultoLog.Message($"looking through folder {text}");
                    string path = Path.Combine(text, directory);
                    VirtualDirectory dir = AbstractFilesystem.GetDirectory(path);
                    if (dir.Exists)
                    {
                        // AultoLog.Message($"loading folder {dir.FullPath}");
                        yield return new SomeModDirectory { dir = dir, mod = mod, folder = text };
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// Loads all the data for a SocietyDef. only needs to be called once.
        /// </summary>
        /// <param name="societyDef"></param>
        public static void Load(SocietyDef societyDef)
        {
            if (DataIsLoaded(societyDef.Key)) return;

            // AultoLog.Message($"Attempting to load data");
            // data isn't loaded, so
            // path --> fileData
            GrammarDatabase.loadedTextFiles[societyDef.Key] = new CaselessDictionary<string, string>();
            //
            DeepProfiler.Start($"{AultoLog.AdvancedPrefix(frame: 1)}  Loading society data: {societyDef.defName} ({societyDef.Key})");
            try
            {
                // AultoLog.Message($"1");
                tmpAlreadyLoadedFiles.Clear();
                
                foreach (SomeModDirectory modDir in GetDirectories(societyDef.FolderPath))
                {
                    // AultoLog.Message($"2");
                    SomeModDirectory localDir = modDir;
                    if (!tmpAlreadyLoadedFiles.ContainsKey(localDir.mod))
                    {
                        // create new element if it doesn't exist already
                        tmpAlreadyLoadedFiles[localDir.mod] = new HashSet<string>();
                    }

                    foreach (VirtualDirectory virtualDir in localDir.dir.GetDirectories("*", SearchOption.TopDirectoryOnly))
                    foreach (VirtualFile virtualFile in virtualDir.GetFiles("*.txt", SearchOption.AllDirectories))
                    {
                        if (TryRegisterFileIfNew(localDir, virtualFile.FullPath, societyDef))
                        {
                            LoadFile(virtualFile, localDir.dir, societyDef);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AultoLog.Error($"Exception loading culture data. Exception: {e}");
            }
            finally
            {
                DeepProfiler.End();
            }

        }

        private static bool TryRegisterFileIfNew(SomeModDirectory modDir, string filePath, SocietyDef society)
        {
#if DEBUG
            // I might not need this
            if (!DataIsLoaded(society.Key))
            {
                AultoLog.Error($"Error executing TryRegisterFileIfNew(...): the key for SocietyDef {society.defName} doesn't exist in GrammarDatabase. It should already exist! something went wrong");
                return false;
            }
#endif
            if (!filePath.StartsWith(modDir.folder))
            {
                AultoLog.Error($"Failed to get a relative path for a file of society {society.defName}: {filePath}, located in {modDir.folder}");
                return false;
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

        private static void LoadFile(VirtualFile file, VirtualDirectory stringsTopDir, SocietyDef societyDef)
        {
            string allText;
            try
            {
                allText = file.ReadAllText();
            }
            catch (Exception ex)
            {
                AultoLog.Error($"Exception from loading strings file {file}: {ex}");
                return;
            }
            string shortPath = file.FullPath;
            if (stringsTopDir != null)
            {
                shortPath = shortPath.Substring(stringsTopDir.FullPath.Length + 1);
            }

            // get the shortened path and replace \ with /
            shortPath = shortPath.Substring(0, shortPath.Length - Path.GetExtension(shortPath).Length).Replace('\\', '/');

            // AultoLog.Message($"loading file {file.FullPath}:  {shortPath}");

            LoadToDatabase(societyDef.Key, shortPath, allText);
        }


        // +------------------------+
        // |     The Variables      |
        // +------------------------+

        // To keep track of files I loaded so I know to skip them.
        private static readonly Dictionary<ModContentPack, HashSet<string>>
            tmpAlreadyLoadedFiles = new Dictionary<ModContentPack, HashSet<string>>();

        public const string fallbackPath = "Languages/English";

        public struct SomeModDirectory
        {
            public VirtualDirectory dir;
            public ModContentPack mod;
            public string folder;
        }
    }
}
