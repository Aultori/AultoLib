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
    public class RaceCommunications
    {
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
            this.cachedReceivers = new Dictionary<string, List<Receiver>>();
            this.cachedTransmitters = new Dictionary<string, List<Transmitter>>();

            foreach (Receiver reciever in this.receivers)
            {
                if (!cachedReceivers.ContainsKey(reciever.medium.defName))
                    cachedReceivers[reciever.medium.defName] = new List<Receiver>();
                cachedReceivers[reciever.medium.defName].Add(reciever);
                reciever.ResolveReferences();
            }

            foreach (Transmitter transmitter in this.transmitters)
            {
                if (!cachedTransmitters.ContainsKey(transmitter.medium.defName))
                    cachedTransmitters[transmitter.medium.defName] = new List<Transmitter>();
                cachedTransmitters[transmitter.medium.defName].Add(transmitter);
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
            return initiator.cachedTransmitters.ContainsKey(medium.defName) && recipient.cachedReceivers.ContainsKey(medium.defName);
        }

        public static (float comfort, float reception, Transmitter transmitter, Receiver reciever, CommunicationLanguageDef language) CommunicationRatings(Pawn initiator, Pawn recipient, CommunicationLanguageDef language) => CommunicationRatings(initiator, recipient, new List<CommunicationLanguageDef> { language }, new List<CommunicationLanguageDef> { language });

        public static (float comfort, float reception, Transmitter transmitter, Receiver reciever, CommunicationLanguageDef language) CommunicationRatings(Pawn initiator, Pawn recipient, List<CommunicationLanguageDef> initiatorLanguages, List<CommunicationLanguageDef> recipientLanguages)
        {
            (float, float, Transmitter, Receiver, CommunicationLanguageDef) errorValue = (0f, 0f, null, null, null);

            // float comfortRating = 0;
            // float receptionRating = 0;
            // CommunicationLanguageDef usedLanguage = null;

            // nested for loops, but the list of known languages is going to be very small, so this won't take long
            foreach (var usedLanguage in initiatorLanguages)
            {
                foreach (var recipientLanguage in recipientLanguages)
                {
                    // usedLanguage = null; // reset this each time
                    // if (usedLanguage.medium == recipientLanguage.medium)
                    if (usedLanguage == recipientLanguage)
                    {
                        // now determine which transmitter and reciever to use

                        if (TryGetBestCommunicationStats(initiator, recipient, usedLanguage, out var stats))
                            return (stats.BestComfort, stats.bestReception, stats.bestTransmitter, stats.bestReciever, usedLanguage);
                        // gets here if communication is impossable
                    }
                }
            }

            //if (usedLanguage == null) return errorValue; // couldn't find any possible communicaion

            return errorValue; // couldn't find any possible communication

            //return (comfortRating, receptionRating, usedLanguage);
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

            if (!initiatorCommunications.cachedTransmitters.TryGetValue(language.medium.defName, out List<Transmitter> transmitters)) return false;
            if (!recipientCommunications.cachedReceivers.TryGetValue(language.medium.defName, out List<Receiver> recievers)) return false;

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
        private Dictionary<string, List<Transmitter>> cachedTransmitters;
        [Unsaved(false)]
        private Dictionary<string, List<Receiver>> cachedReceivers;

        // +---------------+
        // |    Classes    |
        // +---------------+
        public class Transmitter
        {
            public IEnumerable<string> ConfigErrors()
            {
                string prefix = $"[Transmitter {name}]";
                if (medium == null) yield return $"{prefix} {nameof(medium)} cannot be null.";
                if (transmitterWorkerClass == typeof(CommunicationTransmitterWorker)) yield return $"{prefix} {nameof(transmitterWorkerClass)} cannot be null.";
                if (comfortableDistanceCurve == null) yield return $"{prefix} {nameof(comfortableDistanceCurve)} cannot be null.";
            }

            public void ResolveReferences()
            {
                distances = new CommunicationDistances(comfortableDistanceCurve, name);
            }

            public float ComfortRating(Pawn initiator, Pawn reciever) => this.distances.Rating(initiator.Position, reciever.Position);
            public float ComfortRating(IntVec3 initiator, IntVec3  reciever) => this.distances.Rating(initiator, reciever);

            public CommunicationTransmitterWorker Worker => workerInt ?? (workerInt = (CommunicationTransmitterWorker)Activator.CreateInstance(this.transmitterWorkerClass));


            public string                 name;
            public CommunicationMediumDef medium;
            public Type                   transmitterWorkerClass = typeof(CommunicationTransmitterWorker);
            public SimpleCurve            comfortableDistanceCurve;

            [Unsaved(false)] private CommunicationDistances distances;
            [Unsaved(false)] private CommunicationTransmitterWorker workerInt;
        }
        public class Receiver
        {
            public IEnumerable<string> ConfigErrors()
            {
                string prefix = $"[Reciever {name}]";
                if (medium == null) yield return $"{prefix} {nameof(medium)} cannot be null.";
                if (receiverWorkerClass == typeof(CommunicationTransmitterWorker)) yield return $"{prefix} {nameof(receiverWorkerClass)} cannot be null.";
                if (effectivenessCurve == null) yield return $"{prefix} {nameof(effectivenessCurve)} cannot be null.";
            }

            public void ResolveReferences()
            {
                distances = new CommunicationDistances(effectivenessCurve, name);
            }

            public float EffectivenessRating(Pawn initiator, Pawn reciever) => this.distances.Rating(initiator.Position, reciever.Position);
            public float EffectivenessRating(IntVec3 initiator, IntVec3  reciever) => this.distances.Rating(initiator, reciever);

            public CommunicationReceiverWorker Worker => workerInt ?? (workerInt = (CommunicationReceiverWorker)Activator.CreateInstance(this.receiverWorkerClass));

            public string                 name;
            public CommunicationMediumDef medium;
            public Type                   receiverWorkerClass = typeof(CommunicationReceiverWorker);
            public SimpleCurve            effectivenessCurve;

            [Unsaved(false)] private CommunicationReceiverWorker workerInt;
            [Unsaved(false)] private CommunicationDistances distances;
        }
        public class CommunicationDistances
        {
            public CommunicationDistances(SimpleCurve curve, string nameToPrintInError)
            {
                // this was simple, but I want to make sure no points are outside the interval of [ 0.0, 1.0 ]
                // List<CurvePoint> points = comfortableDistanceCurve.Points;
                // List<CurvePoint> pointsSquared = points.ConvertAll(p => new CurvePoint(p.x * p.x, p.y));

                List<CurvePoint> pointsSquared = new List<CurvePoint>();

                foreach (CurvePoint p in curve.Points)
                {
                    float X2 = p.x * p.x;
                    float Y = p.y;
                    if ( Y < 0.0f || 1.0f < Y )
                    {
                        AultoLibMod.Warning($"{nameToPrintInError} has a point with a y value outside the range of [0.0,1.0]");
                        if (Y < 0.0f) Y = 0.0f;
                        if (1.0  < Y) Y = 1.0f;
                    }

                    pointsSquared.Add(new CurvePoint(X2, Y));
                }

                if (pointsSquared.Last().y != 0.0f)
                {
                    AultoLibMod.Warning($"{nameToPrintInError}'s last point doesn't end in 0.0f. There's a chance for the pawn to communicate over an unlimited distance.");
                }

                squaredDistances = new SimpleCurve(pointsSquared);
            }
            public float Rating(IntVec3 transmitter, IntVec3 reciever)
            {
                float x = transmitter.x - reciever.x;
                float y = transmitter.y - reciever.y;
                return squaredDistances.Evaluate( x*x + y*y );
            }

            private readonly SimpleCurve squaredDistances;
        }
    }
}
