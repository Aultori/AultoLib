using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public static class CultureUtil
    {
        /// <summary>
        /// just to get this working.
        /// In the future, I want to have this list generated at runtime so people can easily add their own cultures.
        /// </summary>
        public static List<CultureDef> cultureList = new List<CultureDef> { CultureDefOf.vilos, CultureDefOf.fallback };

        public static Dictionary<Pawn,CultureDef> pawnCultures = new Dictionary<Pawn,CultureDef>();


        /// <summary>
        /// call this in RimVilos to build the word data structures? no, that's automatic
        /// </summary>
        public static void Init()
        {

        }

        /// <summary>
        /// Identifies which culture the pawn should look like.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public static CultureDef CultureOf(Pawn pawn)
        {
            // I could simplify this later
            if (!pawnCultures.ContainsKey(pawn))
            {
                pawnCultures.Add(pawn, FindCultureOf(pawn));
            }

            return pawnCultures[pawn];

            return CultureDefOf.fallback;
        }

        /// <summary>
        /// Determine which culture the pawn should act like.
        /// It searches for it from scratch
        /// </summary>
        /// <param name="pawn">the pawn</param>
        /// <returns>the pawn's CultureDef</returns>
        private static CultureDef FindCultureOf(Pawn pawn)
        {
            CultureDef pawnsCulture = null;

            foreach (CultureDef culture in cultureList)
            {
                if (culture.HasCulture(pawn))
                    pawnsCulture = culture;

#if DEBUG
                Log.Message($"{Globals.DEBUG_LOG_HEADER} adding pawn {pawn.Name}");
#endif
            }

            if (pawnsCulture != null)
                return pawnsCulture;

            return CultureDefOf.fallback;
        }
    }
}
