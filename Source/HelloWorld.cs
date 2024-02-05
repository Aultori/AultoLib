using Verse;

namespace AultoLib
{
    [StaticConstructorOnStartup]
    public static class HelloWorld
    {
        static HelloWorld()
        {
            Log.Message($"{Globals.LOG_HEADER} Hello world!");
            #if DEBUG
            Log.Message($"{Globals.DEBUG_LOG_HEADER} Debug build active!");
            #endif
        }
    }
}
