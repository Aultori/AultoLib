using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.Grammar;
using Verse;
using AultoLib.Database;

namespace AultoLib.Grammar
{
    /// <summary>
    /// Intended to allow selection of a random "rule" based on probabilly and the provided constants.
    /// Does not generate segments
    /// insead validates rules
    /// </summary>
    public class CompoundRule
    {
        // doesn't have this. but it's elements have it.
        // public float SelectionWeight
        // { get { return this.SelectionWeight; } }

        public int RuleCount => this.rules.Count;


        public CompoundRule(string keyword)
        {
            this.keyword = keyword;
            this.rules = new List<ExtendedRule>();
        }

        public void Add(ExtendedRule rule)
        {
            #if DEBUG
            if (rule.keyword != this.keyword)
                Log.Warning($"{Globals.DEBUG_LOG_HEADER} Added an ExtendedRule to a CompoundRule with a mismatching keyword!");
            #endif
            this.rules.Add(rule);
        }
        public void Add(CompoundRule compoundRule)
        {
            #if DEBUG
            if (compoundRule.keyword != this.keyword)
                Log.Warning($"{Globals.DEBUG_LOG_HEADER} Added an CompoundRule to a CompoundRule with mismatching keywords!");
            #endif
            this.rules.AddRange(compoundRule.rules);
        }

        public CompoundRule DeepCopy()
        {
            CompoundRule compoundRule = new CompoundRule(this.keyword);
            compoundRule.rules = this.rules.ConvertAll(entry => entry.DeepCopy());
            return compoundRule;
        }
        public CompoundRule LightCopy()
        {
            CompoundRule compoundRule = new CompoundRule(this.keyword);
            compoundRule.rules = this.rules.ConvertAll(entry => entry);
            return compoundRule;
        }

        public ExtendedRule RandomPossiblyResolvableEntry(HashSet<string> encounteredTags, ICustomizer customizer = null)
        {
            if (this.rules == null) return null;

            if (this.rules.Count == 1)
            {
                if (ResolverInstance.ValidateExtendedRule(this.rules[0], encounteredTags)) return this.rules[0];
                return null;
            }

            List<ExtendedRule> list = this.rules;

            float maxPriority = float.MinValue;
            foreach (ExtendedRule entry in list)
            {
                if (ResolverInstance.ValidateExtendedRule(entry, encounteredTags) && GetSelectionWeight(entry) != 0f)
                {
                    maxPriority = Mathf.Max(maxPriority, entry.Priority);
                }
            }

            IComparer<ExtendedRule> customComparer = customizer?.StrictRulePrioritizer();

            if (customComparer != null && list.Count>1)
            {
                IComparer<ExtendedRule> comparer = Comparer<ExtendedRule>
                    .Create((ExtendedRule a, ExtendedRule b) => customComparer.Compare(a,b))
                    .ThenBy(
                        Comparer<ExtendedRule>
                        .Create((ExtendedRule a, ExtendedRule b) => GetSelectionWeight(a).CompareTo(GetSelectionWeight(b)) )
                        .Descending<ExtendedRule>()
                    );
                tmpSortedList.Clear();
                foreach (ExtendedRule entry in list)
                {
                    if (ResolverInstance.ValidateExtendedRule(entry, encounteredTags) && entry.Priority == maxPriority)
                    {
                        tmpSortedList.Add(entry);
                    }
                }
                tmpSortedList.Shuffle<ExtendedRule>();
                ExtendedRule result;
                tmpSortedList.TryMinBy((ExtendedRule x) => x, comparer, out result);
                return result;
            }

            // Log.Message($"{Globals.DEBUG_LOG_HEADER} picking element by weight");
            // the regular return method thing
            return list.RandomElementByWeightWithFallback(delegate (ExtendedRule rule)
            {
                if (rule.Priority == maxPriority && ResolverInstance.ValidateExtendedRule(rule, encounteredTags))
                    return GetSelectionWeight(rule);
                else
                    return 0f;
            }, null);
        }

        private static float GetSelectionWeight(ExtendedRule rule)
        {

            // Log.Message($"{Globals.DEBUG_LOG_HEADER} getting selection weight");
            return rule.Weight * 100000f / (float)((rule.uses + 1) * 1000);
        }

        // +-----------------+
        // |    Variables    |
        // +-----------------+

        public string keyword;

        // internal List<ExtendedRule> rules;
        public List<ExtendedRule> rules;

        // make sure `rule.ProcessInstance()` is called before calling this.


        private static List<ExtendedRule> tmpSortedList = new List<ExtendedRule>();

        // +------------------+
        // |    Rule Entry    |
        // +------------------+
        // public class RuleEntry
        // {
        //     public float SelectionWeight
        //     {
        //         get
        //         {
        //             return this.rule.Weight * 100000f / (float)((this.uses + 1) * 1000);
        //         }
        //     }

        //     public float Priority { get { return this.rule.Priority; } }


        //     public bool knownUnresolvable;
        //     public int uses = 0;
        //     public ExtendedRule rule;

        // }
    }
}
