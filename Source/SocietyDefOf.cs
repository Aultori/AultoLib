using RimWorld;

namespace AultoLib
{
    [DefOf]
    public static class SocietyDefOf
    {
        static SocietyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SocietyDefOf));
        }

        public static SocietyDef fallback;
    }
}
