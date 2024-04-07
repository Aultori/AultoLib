using Verse;

namespace AultoLib.Database
{
    public static class SocietyDef_Loader
    {
        /// <summary>
        /// Load 
        /// </summary>
        /// <param name="society"></param>
        public static void Load(SocietyDef society)
        {
            if (society.loaded) return;
        }

//         public static void LoadToDatabase(string key, SocietyDef society)
//         {
//             GrammarDatabase.loadedSocietyDefs[key.ToLower()] = society;
//             GrammarDatabase.loadedSocietyDefs[key.ToUpper()] = society;
// #if DEBUG
//             Log.Message($"{Globals.DEBUG_LOG_HEADER} Loaded SocietyDef {society.defName}: {key}");
// #endif
//         }
    }
}
