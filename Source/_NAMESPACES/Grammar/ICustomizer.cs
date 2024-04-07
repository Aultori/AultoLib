using System.Collections.Generic;

namespace AultoLib.Grammar
{
    public interface ICustomizer
    {
        IComparer<ExtendedRule> StrictRulePrioritizer();
        void Notify_RuleUsed(ExtendedRule rule);
        bool ValidateRule(ExtendedRule rule);
    }
}
