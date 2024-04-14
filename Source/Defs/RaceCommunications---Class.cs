using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using AultoLib.CustomProperties;

namespace AultoLib
{
    /// <summary>
    /// The abilites a race uses to communicate with others.
    /// </summary>
    public partial class RaceCommunications
    {
        // /// <summary>
        // /// Tags describing neurological language processing traits: The brain's ability to learn types of alien languages.
        // /// <br/> For instance, if a species is sensitive in the frequencies used in communication, or if their
        // /// brain is able to precieve singals from a medium.
        // /// </summary>
        // public List<string> languageProcessingTraits;

        // this goes in SocietyDef
        // /// <summary>
        // /// preferred languages in order
        // /// </summary>
        // public List<CommunicationLanguageDef> preferredLanguages;

        // /// <summary>
        // /// Languages this race can't learn, most likely due to neurological limitations.
        // /// </summary>
        // public List<CommunicationLanguageDef> disabledLanguages;


        public List<LanguageData> languages;

        /// <summary>
        /// List of communication transmitters starting with the most preferred
        /// </summary>
        public List<Transmitter> transmitters;
        /// <summary>
        /// List of communication receivers
        /// </summary>
        public List<Receiver> receivers;

        // medium defName is used as the key
        [Unsaved(false)]
        private Dictionary<CommunicationMediumDef, List<Transmitter>> cachedTransmitters;
        [Unsaved(false)]
        private Dictionary<CommunicationMediumDef, List<Receiver>> cachedReceivers;
        [Unsaved(false)]
        private Dictionary<CommunicationLanguageDef, LanguageData> cachedLanguages;

        // +---------------+
        // |    Methods    |
        // +---------------+
        public IEnumerable<string> ConfigErrors()
        {
            if (transmitters == null) yield return $"{nameof(transmitters)} is null";
            if (receivers == null) yield return $"{nameof(receivers)} is null";

            foreach (Transmitter transmitter in transmitters)
                foreach (string error in transmitter.ConfigErrors())
                    yield return error;

            foreach (Receiver reciever in receivers)
                foreach (string error in reciever.ConfigErrors())
                    yield return error;
        }

        public void ResolveReferences()
        {
            // load the cached stuff
            this.cachedReceivers = new Dictionary<CommunicationMediumDef, List<Receiver>>();
            this.cachedTransmitters = new Dictionary<CommunicationMediumDef, List<Transmitter>>();

            foreach (LanguageData langData in this.languages)
            {
                if (!cachedLanguages.TryAdd(langData.languageDef, langData))
                    AultoLog.Error($"multiple {nameof(CommunicationLanguageDef)}s present");
            }

            foreach (Receiver reciever in this.receivers)
            {
                if (!cachedReceivers.ContainsKey(reciever.medium))
                    cachedReceivers[reciever.medium] = new List<Receiver>();
                cachedReceivers[reciever.medium].Add(reciever);
                reciever.ResolveReferences();
            }

            foreach (Transmitter transmitter in this.transmitters)
            {
                if (!cachedTransmitters.ContainsKey(transmitter.medium))
                    cachedTransmitters[transmitter.medium] = new List<Transmitter>();
                cachedTransmitters[transmitter.medium].Add(transmitter);
                transmitter.ResolveReferences();
            }
        }

        // public static float DistanceSquared(IntVec3 transmitter, IntVec3 reciever)
        // {
        //     float x = transmitter.x - reciever.x;
        //     float y = transmitter.y - reciever.y;
        //     return x*x + y*y;
        // }

        public static bool ToRename_CanCommunicate(RaceCommunications initiator, RaceCommunications recipient, CommunicationMediumDef medium)
        {
            return initiator.cachedTransmitters.ContainsKey(medium) && recipient.cachedReceivers.ContainsKey(medium);
        }

        // public static (float comfort, float reception, Transmitter transmitter, Receiver reciever, CommunicationLanguageDef language) CommunicationRatings(Pawn initiator, Pawn recipient, CommunicationLanguageDef language) => CommunicationRatings(initiator, recipient, new List<CommunicationLanguageDef> { language }, new List<CommunicationLanguageDef> { language });

        //public static (float comfort, float reception, Transmitter transmitter, Receiver reciever, CommunicationLanguageDef language)
        //    CommunicationRatings(Pawn initiator, Pawn recipient, List<CommunicationLanguageDef> initiatorLanguages, List<CommunicationLanguageDef> recipientLanguages)
        //{
        //    (float, float, Transmitter, Receiver, CommunicationLanguageDef) errorValue = (0f, 0f, null, null, null);
        //    //float comfortRating = 0;
        //    //float receptionRating = 0;
        //    //CommunicationLanguageDef usedLanguage = null;
        //    //  nested for loops, but the list of known languages is going to be very small, so this won't take long
        //    foreach (var usedLanguage in initiatorLanguages)
        //    {
        //        foreach (var recipientLanguage in recipientLanguages)
        //        {
        //            //usedLanguage = null; // reset this each time
        //            //if (usedLanguage.medium == recipientLanguage.medium)
        //            if (usedLanguage == recipientLanguage)
        //            {
        //                // now determine which transmitter and reciever to use
        //                if (TryGetBestCommunicationStats(initiator, recipient, usedLanguage, out var stats))
        //                    return (stats.BestComfort, stats.bestReception, stats.bestTransmitter, stats.bestReciever, usedLanguage);
        //                // gets here if communication is impossable
        //            }
        //        }
        //    }
        //    //if (usedLanguage == null) return errorValue; // couldn't find any possible communicaion
        //    return errorValue; // couldn't find any possible communication
        //    //return (comfortRating, receptionRating, usedLanguage);
        //}

        public static (float comfort, float reception, Transmitter transmitter, Receiver reciever, CommunicationLanguageDef language)
            CommunicationRatings(Pawn initiator, Pawn recipient)
        {
            (float, float, Transmitter, Receiver, CommunicationLanguageDef) errorValue = (0f, 0f, null, null, null);
            //  how to select the best comfort and reception
            //  sometimes, pawns won't be able to communicate at all

            RaceCommunications initiatorCommunications = initiator.RaceProps?.Communications(); 
            RaceCommunications recipientCommunications = recipient.RaceProps?.Communications();

            if (initiatorCommunications == null || recipientCommunications == null) return errorValue;


            float bestComfort = 0.0f;
            float bestReception = 0.0f;
            Transmitter bestTransmitter = null;
            Receiver bestReciever = null;

            //  for each initiator language, determine if the recipient can use that language
            //  then 

            //foreach (var language in initiatorLanguages.Where((x) => recipientLanguages.Contains(x)))
            foreach (var langData in initiatorCommunications.languages)
            {
                if (!recipientCommunications.cachedLanguages.TryGetValue(langData.languageDef, out var recipientLangData)) continue;
                foreach (var medium in langData.mediums.Where( x => recipientLangData.mediums.Contains(x) ))
                {
                    bestReception = 0.0f;
                    bestComfort = 0.0f;
                    bestTransmitter = null;
                    bestReciever = null;

                    if (!initiatorCommunications.cachedTransmitters.TryGetValue(medium, out List<Transmitter> transmitters)) continue;
                    if (!recipientCommunications.cachedReceivers.TryGetValue(medium, out List<Receiver> recievers)) continue;

                    foreach (var reciever in recievers)
                    {
                        float tmp = reciever.EffectivenessRating(initiator.Position, recipient.Position);
                        if (tmp > bestReception)
                            (bestReception, bestReciever) = (tmp, reciever);
                    }

                    if (bestReception < 0.25f) continue; // continue if the pawn can't recieve the communication

                    foreach (var transmitter in transmitters)
                    {
                        float tmp = transmitter.ComfortRating(initiator.Position, recipient.Position);
                        if (tmp > bestComfort)
                            (bestComfort, bestTransmitter) = (tmp, transmitter);
                    }

                    if (bestComfort < 0.25f) continue;
                    // success!
                    // bestTransmitter and bestReciever will never be null
                    return (bestComfort, bestReception, bestTransmitter, bestReciever, langData.languageDef);
                }
            }

            return errorValue;
        }

        /// <summary>
        /// must have<br/>
        /// initiator != recipient<br/>
        /// </summary>
        /// <param name="initiator"></param>
        /// <param name="recipient"></param>
        /// <param name="language"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static bool TryGetBestCommunicationStats(Pawn initiator, Pawn recipient, CommunicationLanguageDef language, out (float BestComfort, float bestReception, Transmitter bestTransmitter, Receiver bestReciever) stats)
        {
            // (float BestComfort, float bestReception, Transmitter bestTransmitter, Reciever bestReciever) thing;
            stats = (0f, 0f, null, null);

            if (initiator == recipient) return false;

            RaceCommunications initiatorCommunications = initiator.RaceProps?.Communications();
            RaceCommunications recipientCommunications = recipient.RaceProps?.Communications();

            if (initiatorCommunications == null || recipientCommunications == null) return false;

            if (!initiatorCommunications.cachedTransmitters.TryGetValue(language.medium, out List<Transmitter> transmitters)) return false;
            if (!recipientCommunications.cachedReceivers.TryGetValue(language.medium, out List<Receiver> recievers)) return false;

            float bestComfort = 0.0f;
            float bestReception = 0.0f;
            Transmitter bestTransmitter = null;
            Receiver bestReciever = null;

            foreach (var transmitter in transmitters)
            {
                float comfort = transmitter.ComfortRating(initiator, recipient);
                if (comfort > bestComfort)
                {
                    bestComfort = comfort;
                    bestTransmitter = transmitter;
                }
            }

            foreach (var reciever in recievers)
            {
                float reception = reciever.EffectivenessRating(initiator, recipient);
                if (reception > bestReception)
                {
                    bestReception = reception;
                    bestReciever = reciever;
                }
            }

            if (bestComfort < 0.25f || bestReception < 0.5f) return false;
            if (bestTransmitter == null || bestReciever == null) return false;

            stats = (bestComfort, bestReception, bestTransmitter, bestReciever);
            return true;
        }

        // +---------------+
        // |    Classes    |
        // +---------------+
    }
}
