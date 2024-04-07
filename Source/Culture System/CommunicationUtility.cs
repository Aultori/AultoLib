using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AultoLib.CustomProperties;

using static AultoLib.AultoLibLogging;

namespace AultoLib
{
    public static class CommunicationUtility
    {
        public static bool CanInitiateAnyInteraction(Pawn pawn, InteractionCategoryDef categoryDef = null)
        {
            return pawn.interactions != null && pawn.Awake() && !pawn.IsBurning() && !pawn.IsCategoryBlocked(categoryDef, isInitiator: true);
        }
        public static bool CanRecieveAnyInteraction(Pawn pawn, InteractionCategoryDef categoryDef = null)
        {
            return pawn.Awake() && !pawn.IsBurning() && !pawn.IsCategoryBlocked(categoryDef, isInitiator: false);
        }

        public static bool CanInitiateSocialInteraction(Pawn pawn)
        {
            //return CommunicationUtility.CanInitiateAnyInteraction(pawn)
            //    && pawn.Society() != null
            //    && !pawn.Downed
            //    && !pawn.InAggroMentalState
            //    && !pawn.IsCategoryBlocked(null, isInitiator: false, isRandomInteraction: true)
            //    && pawn.Faction != null
            //    && pawn.ageTracker.CurLifeStage.canInitiateSocialInteraction;  
            if (!CommunicationUtility.CanInitiateAnyInteraction(pawn))
            {
                if (Logging.DoLog()) Logging.Message("Pawn can't initiate any interaction");
                return false;
            }
            if (! (pawn.Society() != null) )
            {
                Logging.Message("Pawn society was null");
                return false;
            }

            return !pawn.Downed
                && !pawn.InAggroMentalState
                && !pawn.IsCategoryBlocked(null, isInitiator: false, isRandomInteraction: true)
                && pawn.Faction != null
                && pawn.ageTracker.CurLifeStage.canInitiateSocialInteraction;  
        }
        public static bool CanRecieveSocialInteraction(Pawn pawn)
        {
            return CommunicationUtility.CanRecieveAnyInteraction(pawn)
                && pawn.Society() != null
                && !pawn.Downed
                && !pawn.InAggroMentalState;
        }
    }
}
