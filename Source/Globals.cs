using AultoLib.Database;

namespace AultoLib
{
    public static class Globals
    {
        internal static readonly string LOG_HEADER = $"<color=orange>[RimVilos_Core]</color>";
        internal static readonly string DEBUG_LOG_HEADER = $"<color=orange>[RimVilos_Core <color=aqua>Debug</color>]</color>";

        public static readonly GrammarDatabase grammarDatabase = new GrammarDatabase();
        public static readonly CommunicationDatabase communicationDatabase = new CommunicationDatabase();
    }
}
