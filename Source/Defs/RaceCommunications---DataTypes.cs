using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Verse;

namespace AultoLib
{
    public partial class RaceCommunications
    {
        public class LanguageData
        {
            public CommunicationLanguageDef languageDef;
            public List<CommunicationMediumDef> mediums; // preferred starts first
        }
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
                        AultoLog.Warning($"{nameToPrintInError} has a point with a y value outside the range of [0.0,1.0]");
                        if (Y < 0.0f) Y = 0.0f;
                        if (1.0  < Y) Y = 1.0f;
                    }

                    pointsSquared.Add(new CurvePoint(X2, Y));
                }

                if (pointsSquared.Last().y != 0.0f)
                {
                    AultoLog.Warning($"{nameToPrintInError}'s last point doesn't end in 0.0f. There's a chance for the pawn to communicate over an unlimited distance.");
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
