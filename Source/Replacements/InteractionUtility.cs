using RimWorld;
using Verse;


namespace AultoLib.Replacements
{
    /// <summary>
    /// Culture variant of RimWorld.InteractionUtility
    /// </summary>
    public static class InteractionUtility
    {
        /// <summary>
        /// Culture varient of RimWorld.CanInitiateInteraction.
        /// Checks if a pawn has the ability to initiate a certian interaction.
        /// </summary>
        /// <param name="pawn"></param>
        /// <param name="interactionDef"></param>
        /// <param name="interactionType"></param>
        /// <returns>true if pawn can initate interacton</returns>
        public static bool CanInitiateInteraction(Pawn pawn, InteractionDef interactionDef = null, CommunicationMethod interactionMethod = null)
        {
            return pawn.interactions != null
                && interactionMethod.PawnCanInitiate(pawn)
                && !pawn.IsInteractionBlocked(interactionDef, true, false);
        }


    }
}
