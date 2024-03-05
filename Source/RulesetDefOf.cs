using RimWorld;

namespace AultoLib
{
    [DefOf]
    public class RulesetDefOf
    {
        static RulesetDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SocietyDefOf));
        }

        public static RulesetDef GlobalUtility_Fallback;

        public static RulesetDef TalkTopicsUtility_Fallback;
    }
}
