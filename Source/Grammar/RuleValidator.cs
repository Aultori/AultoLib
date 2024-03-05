using System.Collections.Generic;
using Verse;
using AultoLib.Database;

namespace AultoLib.Grammar
{
    public static class RuleValidator
    {

        public static bool ValidateRule(ExtendedRule rule, ICustomizer customizer = null)
        {
            rule.ProcessInstance(ResolverData.runID);
            return !rule.knownUnresolveable
                && ValidateConstantConstraints(rule)
                && ValidateRequiredTag(rule)
                && ValidateTimesUsed(rule)
                && customizer?.ValidateRule(rule) == true;
        }

        private static bool ValidateConstantConstraints(ExtendedRule rule)
        {
            if (!rule.constraintsChecked)
            {
                rule.constraintsValid = rule.Constraints.Test(ResolverData.localConstants);
                rule.constraintsChecked = true;
            }
            return rule.constraintsValid;
        }

        private static bool ValidateRequiredTag(ExtendedRule rule)
        {
            return rule.requiredTag.NullOrEmpty()
                || ResolverData.extraTags?.Contains(rule.requiredTag) == true
                || ResolverData.resolvedTags?.Contains(rule.requiredTag) == true;
        }

        private static bool ValidateTimesUsed(ExtendedRule rule)
        {
            return rule.usesLimit == null || rule.uses < rule.usesLimit;
        }
    }
}
