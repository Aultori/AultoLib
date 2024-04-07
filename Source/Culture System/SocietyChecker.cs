using Verse;

namespace AultoLib
{
    /// <summary>
    /// Facilitates society detection.
    /// </summary>
    public abstract class SocietyChecker
    {
        public virtual bool Check(Pawn pawn)
        {
            return false;
        }
    }
}
