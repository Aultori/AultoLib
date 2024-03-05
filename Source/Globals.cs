using AultoLib.Database;

namespace AultoLib
{
    public static class Globals
    {
        internal static readonly string LOG_HEADER = $"<color=orange>[AultoLib]</color>";
        internal static readonly string DEBUG_LOG_HEADER = $"<color=orange>[AultoLib <color=aqua>Debug</color>]</color>";
        internal static readonly string RESOLVER_HEADER = $"<color=orange>[AultoLib:Resolver]</color>";

        public const string FALLBACK_SOCIETY_KEY = "FALLBACK";
        public const string INSTANCE_SOCIETY_KEY = "INSTANCE";
        public static string ACTIVE_SOCIETY_KEY = "FALLBACK"; // value can change
        public static readonly CommunicationDatabase communicationDatabase = new CommunicationDatabase();
    }
}
