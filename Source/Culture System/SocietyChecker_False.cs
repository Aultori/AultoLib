using Verse;

namespace AultoLib
{
    public class SocietyChecker_False : SocietyChecker
    {
        public override bool Check(Pawn pawn)
        {
            return false;
        }
    }
}
