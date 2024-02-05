using RimWorld;

namespace AultoLib
{
    [DefOf]
    public static class CultureDefOf
    {
        static CultureDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(CultureDefOf));
        }

        public static CultureDef fallback;

        public static CultureDef any;

        public static CultureDef vilos;
    }
}
