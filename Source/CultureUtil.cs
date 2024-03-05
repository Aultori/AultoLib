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
      //   public static List<SocietyDef> cultureList = new List<SocietyDef> { SocietyDefOf.vilos, CultureDefOf.fallback };

        public static Dictionary<Pawn,SocietyDef> pawnCultures = new Dictionary<Pawn,SocietyDef>();


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
        public static SocietyDef CultureOf(Pawn pawn)
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
        /// <returns>the pawn's SocietyDef</returns>
        private static SocietyDef FindCultureOf(Pawn pawn)
        {
            SocietyDef pawnsCulture = null;

            foreach (SocietyDef culture in cultureList)
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
