using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace AultoLib
{
    public static class SocialFightUtility
    {
        public static bool CheckSocialFightStart(InteractionInstanceDef intInstDef, Pawn initiator, Pawn recipient)
        {
            if (!DebugSettings.enableRandomMentalStates) return false;
            if (recipient.needs.mood == null || TutorSystem.TutorialMode) return false;
            if (DebugSettings.alwaysSocialFight || Rand.Value < RimWorld_SocialFightChance(intInstDef, initiator, recipient))
            {
                StartSocialFight(initiator, recipient);
                return true;
            }
            return false;
        }

        public static void StartSocialFight(Pawn initiator, Pawn recipient, string messageKey = "MessageSocialFight")
        {
            if (PawnUtility.ShouldSendNotificationAbout(initiator) || PawnUtility.ShouldSendNotificationAbout(recipient))
            {
                string messageText = messageKey.Translate(initiator.LabelShort, recipient.LabelShort, initiator.Named("PAWN1"), recipient.Named("PAWN2"));
                Messages.Message(messageText, initiator, RimWorld.MessageTypeDefOf.ThreatSmall, historical: true);
            }
            initiator.mindState.mentalStateHandler.TryStartMentalState(RimWorld.MentalStateDefOf.SocialFighting, otherPawn: recipient);
            recipient.mindState.mentalStateHandler.TryStartMentalState(RimWorld.MentalStateDefOf.SocialFighting, otherPawn: initiator);
            TaleRecorder.RecordTale(RimWorld.TaleDefOf.SocialFight, new object[] { initiator, recipient });
        }

        // will be more complicated later
        // this is the one from RimWorld:
        public static bool SocialFightPossible(Pawn insulter, Pawn recipient)
        {
            if (!insulter.RaceProps.Humanlike || !recipient.RaceProps.Humanlike) return false;
            if (!InteractionUtility.HasAnyVerbForSocialFight(insulter) || !InteractionUtility.HasAnyVerbForSocialFight(recipient)) return false;
            if (recipient.WorkTagIsDisabled(WorkTags.Violent)) return false;
            if (insulter.Downed || recipient.Downed) return false;
            if (recipient.IsSlave && !insulter.IsSlave) return false;

            DevelopmentalStage developmentalStage = recipient.ageTracker.CurLifeStage.developmentalStage;
            int ageDifference = Mathf.Abs(recipient.ageTracker.AgeBiologicalYears - insulter.ageTracker.AgeBiologicalYears);

            if (developmentalStage == DevelopmentalStage.Baby) return false;
            if (ageDifference > 6 && developmentalStage != DevelopmentalStage.Child) return false;
            if ((insulter.genes?.SocialFightChanceFactor ?? 1f) <= 0f) return false; // if there are no genes for SocialFightChanceFactor, there's a factor by default
            if ((recipient.genes?.SocialFightChanceFactor ?? 1f) <= 0f) return false;

            return true;
        }


        // Same functionallity as the default method
        public static float RimWorld_SocialFightChance(InteractionInstanceDef intInstDef, Pawn initiator, Pawn victim)
        {
            if (intInstDef == null || initiator == null || victim == null) return 0f;
            if (!SocialFightPossible(initiator, victim)) return 0f;

            float chance = intInstDef.socialFightBaseChance;
            chance *= Mathf.InverseLerp(0.3f, 1f, victim.health.capacities.GetLevel(RimWorld.PawnCapacityDefOf.Manipulation));
            chance *= Mathf.InverseLerp(0.3f, 1f, victim.health.capacities.GetLevel(RimWorld.PawnCapacityDefOf.Moving));
            foreach (Hediff hediff in victim.health.hediffSet.hediffs.Where(h => h!=null))
            {
                chance *= hediff.CurStage.socialFightChanceFactor;
            }

            float opinionOfInsulter = victim.relations.OpinionOf(initiator);
            if (opinionOfInsulter < 0f)
            {
                chance *= GenMath.LerpDouble(-100f, 0f, 4f, 1f, opinionOfInsulter);
            }
            else
            {
                chance *= GenMath.LerpDouble(0f, 100f, 1f, 0.6f, opinionOfInsulter);
            }

            if (victim.RaceProps.Humanlike)
            {
                foreach (Trait trait in victim.story.traits.allTraits)
                {
                    if (!trait.Suppressed) chance *= trait.CurrentData.socialFightChanceFactor;
                }
            }

            int ageDiffence = Mathf.Abs(victim.ageTracker.AgeBiologicalYears - initiator.ageTracker.AgeBiologicalYears);
            if (ageDiffence > 10)
            {
                if (ageDiffence > 50) ageDiffence = 50;
                chance *= GenMath.LerpDouble(10f, 50f, 1f, 0.25f, (float)ageDiffence);
            }
            if (victim.IsSlave) chance *= 0.5f;
            if (victim.genes != null) chance *= victim.genes.SocialFightChanceFactor;
            if (initiator.genes != null) chance *= initiator.genes.SocialFightChanceFactor;

            return Mathf.Clamp01(chance);
        }
    }
}
