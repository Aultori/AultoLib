using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class CommunicationTransmitterDef : Def
    {
        // +---------------------+
        // |    Verse Loading    |
        // +---------------------+
        public override IEnumerable<string> ConfigErrors()
        {
            foreach (string error in base.ConfigErrors()) yield return error;

            if (medium == null) yield return $"{nameof(medium)} cannot be null.";
            if (transmitterWorkerClass == typeof(CommunicationTransmitterWorker)) yield return $"{nameof(transmitterWorkerClass)} cannot be null.";
            if (comfortableDistanceCurve == null) yield return $"{nameof(comfortableDistanceCurve)} cannot be null.";
        }

        public override void PostLoad()
        {
            // this was simple, but I want to make sure no points are outside the interval of [ 0.0, 1.0 ]
            // List<CurvePoint> points = comfortableDistanceCurve.Points;
            // List<CurvePoint> pointsSquared = points.ConvertAll(p => new CurvePoint(p.x * p.x, p.y));

            List<CurvePoint> pointsSquared = new List<CurvePoint>();

            foreach (CurvePoint p in comfortableDistanceCurve.Points)
            {
                float X2 = p.x * p.x;
                float Y = p.y;
                if ( Y < 0.0f || 1.0f < Y )
                {
                    AultoLibMod.Warning($"{this.defName} has a point with a y value outside the range of [0.0,1.0]");
                    if (Y < 0.0f) Y = 0.0f;
                    if (1.0  < Y) Y = 1.0f;
                }

                pointsSquared.Add(new CurvePoint(X2, Y));
            }

            if (pointsSquared.Last().y != 0.0f)
            {
                AultoLibMod.Warning($"{this.defName}'s last point doesn't end in 0.0f. There's a chance for the pawn to communicate over an unlimited distance.");
            }

            squaredDistances = new SimpleCurve(pointsSquared);
        }

        // +---------------+
        // |    Methods    |
        // +---------------+

        public CommunicationTransmitterWorker RecieverWorker
        {
            get
            {
                if (this.workerInt == null)
                {
                    this.workerInt = (CommunicationTransmitterWorker)Activator.CreateInstance(this.transmitterWorkerClass);
                }
                return this.workerInt;
            }
        }


        public float DistanceRating(Pawn transmitter, Pawn reciever)
        {
            return this.DistanceRating(transmitter.Position, reciever.Position);
        }

        public float DistanceRating(IntVec3 transmitter, IntVec3 reciever)
        {
            float x = transmitter.x - reciever.x;
            float y = transmitter.y - reciever.y;
            return squaredDistances.Evaluate( x*x + y*y );
        }

        // +-----------------+
        // |    Variables    |
        // +-----------------+
        public CommunicationMediumDef medium; // something like auditory or sound

        // capacity is included in the transmitter Worker
        // public PawnCapacityDef capacity;

        public Type transmitterWorkerClass = typeof(CommunicationTransmitterWorker);

        /// <summary>
        /// The distrubution of distances the speaker is comfortable having conversations at. <br/>
        /// <i>y points must be within the interval of [0.0,1.0]</i>
        /// </summary>
        public SimpleCurve comfortableDistanceCurve;

        [Unsaved(false)]
        private CommunicationTransmitterWorker workerInt;

        // This way I won't have to use square root every time I want to find the distance rating.
        [Unsaved(false)]
        private SimpleCurve squaredDistances;

        // handle loudness and reception stuff later.
    }
}
