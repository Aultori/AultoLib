using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// Similar to <see cref="RimWorld.Pawn_InteractionsTracker"/>
    /// </summary>
    public static class Pawn_InteractionInstancesTracker
    {

        /// <summary>
        /// Returns the communication method the initiator <see cref="Pawn"/> will use.
        /// </summary>
        public static bool TryGetCommunicationMethod(Pawn initiator, Pawn recipient, float distance, out CommunicationMethod method)
        {
            // a pawn's race contains the possible communication methods
            CommunicationCapacities initiatorCapacities = initiator.RaceProps.GetCommunicationCapacities();
            CommunicationCapacities recipientCapacities = recipient.RaceProps.GetCommunicationCapacities();

            // test preferred communicationMethods, then possible ones
            foreach (CommunicationMethod commMethod in initiatorCapacities.list)
            {
                commMethod
            }


            // if a preferred communication method was found, return it.





            method = null;
            return false;
        }
    }
}
