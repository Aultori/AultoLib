using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using AultoLib.CustomProperties;

namespace AultoLib.Database
{
    public static class SocietyDatabase
    {
        // public static SocietyDef GetSocietyDef(string society_key)
        // {
        //     if (TryGetSocietyDef(society_key, out SocietyDef def))
        //         return def;
        //     return SocietyDefOf.fallback;
        // }

        // // everything is fallback for now
        // public static SocietyDef Society(this Pawn pawn)
        // {
        //     // if (pawnSocieties.TryGetValue(pawn, out SocietyDef def)) return def;
        //     if (pawnSocieties.TryGetValue(pawn, out SocietyDef def))
        //     {
        //         if (def is SocietyDef def2)
        //             return def2;
        //         return null;
        //     }
        //     // get society
        //     def = SocietyDefOf.fallback;
        //     // TODO: when checking for a pawn's society. I should check all SocietyDefs in reverse order from which they were loaded. that way mods can override things
        //     // foreach (SocietyDef societyDef in DefDatabase<SocietyDef>.AllDefs)
        //     foreach (SocietyDef societyDef in GrammarDatabase.loadedSocietyDefs.Values)
        //     {
        //         // AultoLog.DebugMessage($"checking societyDef {societyDef.defName}");
        //         if (societyDef.HasSociety(pawn))
        //         {
        //             def = societyDef;
        //             break;
        //         }
        //     }
        //     pawnSocieties.SetValue(pawn, def);
        //     // AddPawn(pawn, def);
        //     return def;
        //     // return SocietyDefOf.fallback;
        // }

        public static void AddSociety(SocietyDef societyDef)
        {
            societyDefs[societyDef.Key] = societyDef;
        }

        // public static bool TryGetSocietyDef(string society_key, out SocietyDef def)
        //     => societyDefs.TryGetValue(society_key, out def);

        public static Dictionary<string, SocietyDef> societyDefs
            = new Dictionary<string, SocietyDef>();
        // = new Dictionary<string, SocietyDef> { {Globals.FALLBACK_SOCIETY_KEY, SocietyDefOf.fallback } };


        // +---------------------+
        // |    Testing stuff    |
        // +---------------------+

        //public static void AddPawn(Pawn pawn, string society)
        //{
        //    if (TryGetSocietyDef(society, out SocietyDef def))
        //        pawnSocieties[pawn] = def;
        //}
        //public static void AddPawn(Pawn pawn, SocietyDef societyDef)
        //{
        //    pawnSocieties[pawn] = societyDef;
        //}

        
        private static CustomProperty<Pawn,SocietyDef> pawnSocieties = new CustomProperty<Pawn, SocietyDef>();

        // public static Dictionary<Pawn, SocietyDef> pawnSocieties = new Dictionary<Pawn, SocietyDef>();
        // make the fallback be already added?
    }
}
