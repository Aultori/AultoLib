using AultoLib.Database;
using Verse;

namespace AultoLib
{
    [StaticConstructorOnStartup]
    public static class AultoLib_Init
    {
        static AultoLib_Init()
        {
            Log.Message($"{Globals.LOG_HEADER} Hello world!");
            #if DEBUG
            Log.Message($"{Globals.DEBUG_LOG_HEADER} Debug build active!");
#endif

            // It Works !!!
            // Log.Message($"{Globals.LOG_HEADER} TextFiles:");
            // foreach ( var societyData in GrammarDatabase.loadedTextFiles )
            // {
            //     foreach (var rawData in societyData.Value )
            //     {
            //         Log.Message($"{Globals.DEBUG_LOG_HEADER} {rawData.Key} -> {rawData.Value}");
            //     }
            // }
        }
    }
}
