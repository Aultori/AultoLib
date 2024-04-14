using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AultoLib.CustomProperties;
using AultoLib.Database;
using UnityEngine;
using Verse.AI;

using static AultoLib.AultoLog;

namespace AultoLib
{
    public class AultoLib_Pawn_InteractionsTracker
    {
        private static RandomSocialMode CurrentSocialMode
        {
            get
            {
                if (!InteractionUtility.CanInitiateRandomInteraction(pawn))
                {
                    return RandomSocialMode.Off;
                }
                RandomSocialMode randomSocialMode = RandomSocialMode.Normal;
                JobDriver curDriver = pawn.jobs.curDriver;
                if (curDriver != null)
                {
                    randomSocialMode = curDriver.DesiredSocialMode();
                }
                PawnDuty duty = pawn.mindState.duty;
                if (duty != null && duty.def.socialModeMax < randomSocialMode)
                {
                    randomSocialMode = duty.def.socialModeMax;
                }
                if (pawn.Drafted && randomSocialMode > RandomSocialMode.Quiet)
                {
                    randomSocialMode = RandomSocialMode.Quiet;
                }
                if (pawn.InMentalState && randomSocialMode > pawn.MentalState.SocialModeMax())
                {
                    randomSocialMode = pawn.MentalState.SocialModeMax();
                }
                return randomSocialMode;
            }
        }

        public static void InteractionsTrackerTick()
        {
            currentSocialMode = AultoLib.AultoLib_Pawn_InteractionsTracker.CurrentSocialMode;
            if (currentSocialMode == RandomSocialMode.Off)
            {
                wantsRandomInteract = false;
                return;
            }
            if (currentSocialMode == RandomSocialMode.Quiet)
            {
                wantsRandomInteract = false;
            }
            if (!wantsRandomInteract)
            {
                if (Find.TickManager.TicksGame > lastInteractionTime + const_RandomInteractIntervalMin && pawn.IsHashIntervalTick(60))
                {
                    int num = 0;
                    switch (currentSocialMode)
                    {
                    case RandomSocialMode.Quiet:
                        num = const_RandomInteractMTBTicks_Quiet;
                        break;
                    case RandomSocialMode.Normal:
                        num = const_RandomInteractMTBTicks_Normal;
                        break;
                    case RandomSocialMode.SuperActive:
                        num = const_RandomInteractMTBTicks_SuperActive;
                        break;
                    }
                    if (Rand.MTBEventOccurs((float)num, 1f, const_RandomInteractCheckInterval) && !TryInteractSocially())
                    {
                        wantsRandomInteract = true;
                        return;
                    }
                }
            }
            else if (pawn.IsHashIntervalTick(91) && TryInteractSocially())
            {
                wantsRandomInteract = false;
            }
        }


        public static bool InteractedTooRecentlyToInteract() => Find.TickManager.TicksGame < lastInteractionTime + const_InteractIntervalAbsoluteMin;



        private static string NameTag(Pawn pawn)
        {
            //return pawn.ToString().ApplyTag(TagType.Name, null).Resolve();
            return pawn.ToString().ApplyTag(TagType.Name).Resolve();
        }

        public static bool TryInteract(Pawn initiator, Pawn recipient, InteractionCategoryDef category, CommunicationLanguageDef language)
        {
            if (AultoLog.DoLog()) AultoLog.DebugMessage($"{NameTag(initiator)} {initiator.Society()} tries to interact with {recipient.ToString().ApplyTag(TagType.Name)} {recipient.Society()}. {category} language: {language}");

            if (DebugSettings.alwaysSocialFight)
            {
                category = CategoryDefOf.Insult;
            }
            if (initiator == recipient)
            {
                AultoLog.Warning($"{initiator} tried to interact with self. category={category}");
                return false;
            }
            if (initiator.IsCategoryBlocked(category, isInitiator: true) || recipient.IsCategoryBlocked(category, isInitiator: false))
            {
                return false;
            }
            if (!category.ignoreTimeSinceLastInteraction && InteractedTooRecentlyToInteract())
            {
                AultoLog.Error($"{initiator} tried to do interaction category '{category.ColorText("yellow")}' to {recipient} only {Find.TickManager.TicksGame - lastInteractionTime} ticks since last interaction {lastInteraction.ToStringSafe()} (min is {const_InteractIntervalAbsoluteMin})");
                return false;
            }

            // get the interactionInstanceDef
            if (!GrammarDatabase.TryGetInteractionInstance(category.defName, initiator.Society().defName, recipient.Society().defName, out InteractionInstanceDef intInst))
            {
                return false;
            }

            if (intInst.initiatorThought     != null) AddInteractionThought(initiator, recipient, intInst.initiatorThought);
            if (intInst.recipientThought     != null) AddInteractionThought(recipient, initiator, intInst.recipientThought);
            if (intInst.initiatorXpGainSkill != null) initiator.skills.Learn(intInst.initiatorXpGainSkill, intInst.initiatorXpGainAmount);
            if (intInst.recipientXpGainSkill != null) recipient.skills.Learn(intInst.recipientXpGainSkill, intInst.recipientXpGainAmount);

            bool isFight = (recipient.RaceProps.Humanlike && recipient.Spawned)
                && SocialFightUtility.CheckSocialFightStart(intInst, initiator, recipient);

            List<RulesetDef> list = new List<RulesetDef>();
            string      text;
            string      letterLabel;
            LetterDef   letterDef;
            LookTargets lookTargets;

            if (!isFight)
            {
                (text, letterLabel, letterDef, lookTargets) = intInst.Worker.Interacted(initiator, recipient, list);
            }
            else
            {
                (text, letterLabel, letterDef, lookTargets) = (null, null, null, null);
                list.Add(RulesetDefOf.Sentence_SocialFightStarted);
            }

            MoteMaker.MakeInteractionBubble(initiator, recipient, intInst.interactionMote, intInst.GetSymbol(initiator.Faction, initiator.Ideo), intInst.GetSymbolColor(initiator.Faction));

            lastInteractionTime = Find.TickManager.TicksGame;
            lastInteraction = intInst.defName;

            Grammar.Constants extraConstants = new Grammar.Constants();

            string communicationPrefix = language.CommunicationPrefix();
            if (communicationPrefix != null)
            {
                extraConstants.AddConstant("communicationPrefix", communicationPrefix);
            }

            PlayLogEntry_InteractionInstance playLogEntry_InteractionInstance = new PlayLogEntry_InteractionInstance(intInst, initiator, recipient, extraRulesets: list);
            Find.PlayLog.Add(playLogEntry_InteractionInstance);
            if (letterDef != null)
            {
                string letterText = playLogEntry_InteractionInstance.ToGameStringFromPOV(initiator, forceLog: false);
                if (!text.NullOrEmpty())
                {
                    letterText += "\n\n" + text;
                }
                Find.LetterStack.ReceiveLetter(letterLabel, letterText, letterDef, lookTargets ?? initiator);
            }

            return true;
        }

        public static void AddInteractionThought(Pawn initiator, Pawn recipient, ThoughtDef thoughtDef) => RimWorld.Pawn_InteractionsTracker.AddInteractionThought(initiator, recipient, thoughtDef);

        // like RimWorld's TryInteractRandomly
        private static bool TryInteractSocially()
        {
            //AultoLibMod.DebugMessage("Trying to interact socially");

            if (InteractedTooRecentlyToInteract())
            {
                AultoLog.DebugWarning("Interacted too recently.");
                return false;
            }

            if (!CommunicationUtility.CanInitiateSocialInteraction(pawn))
            {
                AultoLog.DebugWarning("Can't initiate social interactions.");
                return false;
            }

            // List<Pawn> mapPawnsInFaction = new List<Pawn>(pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction));
            // AultoLib_Pawn_InteractionsTracker.workingList.Clear();
            // AultoLib_Pawn_InteractionsTracker.workingList.AddRange(pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction));
            // AultoLib_Pawn_InteractionsTracker.workingList.Shuffle<Pawn>();

            // If there are too many pawns in the map, try to interact randomly.
            // if (mapPawnsInFaction.Count >= 64)
            // {
            //     return TryInteractSociallyWithRandomPawn(mapPawnsInFaction);
            // }
            // // sort in order
            // List<PossibleRecipient> listToSort = new List<PossibleRecipient>();

            // foreach (Pawn p in mapPawnsInFaction)
            // {
            //     if (CommunicationUtility.CanRecieveSocialInteraction(p))
            //     {
            //         var ratings = RaceCommunications.CommunicationRatings(pawn, p, pawn.KnownLanguages(), p.KnownLanguages());
            //         if (ratings.comfort != 0f && ratings.reception != 0f)
            //             listToSort.Add(new PossibleRecipient(p, ratings));
            //     }
            // }
            // listToSort.Sort(delegate(PossibleRecipient a, PossibleRecipient b)
            // {
            //     float weightA = a.SelectionWeight();
            //     float weightB = b.SelectionWeight();
            //     if (weightA > weightB) return 1;
            //     if (weightA == weightB) return 0;
            //     return -1;
            // });
            // List<PossibleRecipient> tmp = listToSort.FindAll(delegate(PossibleRecipient e) {
            //     return e.SelectionWeight() > .75 * listToSort.First().SelectionWeight();
            // });
            // tmp.Shuffle();
            // foreach (PossibleRecipient a in tmp)
            // {
            // }

            List<Pawn> mapPawnsInFaction;
            mapPawnsInFaction = new List<Pawn>(pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction));
            mapPawnsInFaction.Shuffle<Pawn>();

            if (AultoLog.DoLog()) AultoLog.DebugMessage("Trying to find a pawn to interact with.");

            foreach (Pawn recipient in mapPawnsInFaction)
            {
                if (recipient == pawn) continue;
                if (recipient.Society() == null) continue;

                // determine communication stats
                (float comfort, float reception, RaceCommunications.Transmitter transmitter, RaceCommunications.Receiver reciever, CommunicationLanguageDef language)
                    = RaceCommunications.CommunicationRatings(pawn, recipient);

                //if (comfort >= (new System.Random()).NextDouble()) continue; // recipient wasn't in range or initiator chose not to interact
                if (comfort < .75f) continue; 

                if (!pawn.IsCarryingPawn(recipient))
                {
                    if (!recipient.Spawned) continue;
                    if (!transmitter.medium.Worker.CommunicationCanReach(pawn, recipient)) continue;
                }
                if (!transmitter.Worker.PawnCanTransmitCommunication(pawn) || !reciever.Worker.PawnCanRecieveCommunication(recipient)) continue; // senses
                if (!CommunicationUtility.CanInitiateAnyInteraction(pawn) || !CommunicationUtility.CanRecieveAnyInteraction(recipient)) continue;

                if (!CommunicationUtility.CanRecieveSocialInteraction(recipient)) continue;

                // todo: this is temporary
                // selecting random category for now
                InteractionCategoryDef category = DefDatabase<InteractionCategoryDef>.AllDefsListForReading.RandomElement();

                if (TryInteract(pawn, recipient, category, language))
                {
                    return true;
                }
                else AultoLog.Error($"{pawn} failed to interact with {recipient}");
            }
            // // randomized map pawns
            // foreach (Pawn recipient in mapPawnsInFaction)
            // {
            //     if (recipient == pawn) continue;
            //     if (recipient.Society() == null) continue;
            //     // determine communication stats
            //     (float comfort, float reception, RaceCommunications.Transmitter transmitter, RaceCommunications.Receiver reciever, CommunicationLanguageDef language)
            //         = RaceCommunications.CommunicationRatings(pawn, recipient, pawn.RaceProps.Languages(), recipient.RaceProps.Languages());
            //     if (comfort >= (new System.Random()).NextDouble()) continue; // recipient wasn't in range or initiator chose not to interact
            //     if (!pawn.IsCarryingPawn(recipient))
            //     {
            //         if (!recipient.Spawned) continue;
            //         if (!transmitter.medium.Worker.CommunicationCanReach(pawn, recipient)) continue;
            //     }
            //     if (!transmitter.Worker.PawnCanTransmitCommunication(pawn) || !reciever.Worker.PawnCanRecieveCommunication(recipient)) continue; // senses
            //     if (!CommunicationUtility.CanInitiateAnyInteraction(pawn) || !CommunicationUtility.CanRecieveAnyInteraction(recipient)) continue;
            //     if (!CommunicationUtility.CanRecieveSocialInteraction(recipient)) continue;

            //     // todo: this is temporary
            //     // selecting random category for now
            //     InteractionCategoryDef category = DefDatabase<InteractionCategoryDef>.AllDefsListForReading.RandomElement();
            //     if (TryInteract(pawn, recipient, category, language))
            //     {
            //         return true;
            //     }
            //     else AultoLibMod.Error($"{pawn} failed to interact with {recipient}");
            // }

            //AultoLibMod.DebugError("Didn't find a pawn to interact with");
            return false;
        }


        readonly struct PossibleRecipient
        {
            public PossibleRecipient(Pawn recipient, (float comfort, float reception, RaceCommunications.Transmitter transmitter, RaceCommunications.Receiver reciever, CommunicationLanguageDef language) data)
            {
                this.pawn = recipient;
                this.comfort = data.comfort;
                this.reception = data.reception;
                this.transmitter = data.transmitter;
                this.reciever = data.reciever;
                this.language = data.language;
            }

            public float SelectionWeight()
            {
                return 2*comfort + reception;
            }

            public readonly Pawn pawn;
            public readonly float comfort;
            public readonly float reception;
            public readonly RaceCommunications.Transmitter transmitter;
            public readonly RaceCommunications.Receiver reciever;
            public readonly CommunicationLanguageDef language;
        }

        public static RandomSocialMode currentSocialMode;

        public static void LoadPawn(Pawn pawn)
        {
            AultoLib_Pawn_InteractionsTracker.pawn = pawn;
        }
        public static void LoadWantsRandomInteract(bool param)
        {
            wantsRandomInteract = param;
        }
        public static void LoadLastInteractionTime(int param)
        {
            lastInteractionTime = param;
        }
        public static void LoadLastInteraction(string param)
        {
            lastInteraction = param;
        }

        // Vars from default
        public static Pawn pawn;
        public static bool wantsRandomInteract;
        public static int lastInteractionTime = -9999;
        public static string lastInteraction;
        // private const int   const_RandomInteractMTBTicks_Quiet = 22000;
        // private const int   const_RandomInteractMTBTicks_Normal = 6600;
        private const int   const_RandomInteractMTBTicks_Quiet =  550;
        private const int   const_RandomInteractMTBTicks_Normal = 550;
        private const int   const_RandomInteractMTBTicks_SuperActive = 550;
        public  const int   const_RandomInteractIntervalMin = 320;
        private const int   const_RandomInteractCheckInterval = 60;
        private const float const_SlaveSocialFightFactor = 0.5f;
        private const int   const_ChildSocialFightAgeRange = 6;
        private const int   const_InteractIntervalAbsoluteMin = 120;
        public  const int   const_DirectTalkInteractInterval = 320;
        public  const float const_IdeoExposurePointsInteraction = 0.5f;

        private static List<Pawn> workingList; // set this to the 
    }
}
