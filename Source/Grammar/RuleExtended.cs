using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.Grammar;

namespace AultoLib.Grammar
{
    /// <summary>
    /// a new data structure is included in the rule to allow for quick string building
    /// </summary>
    public class RuleExtended : Rule
    {
        public override float BaseSelectionWeight
        { get { return this.weight; } }
        
        public override float Priority
        { get { return this.priority; } }

        public override string Generate()
        { return this.output; }

        public override Rule DeepCopy()
        {
            RuleExtended ruleExtended = (RuleExtended)base.DeepCopy();
            ruleExtended.output = this.output;
            ruleExtended.weight = this.weight;
            ruleExtended.priority = this.priority;
            ruleExtended.segments = GenerateOutputSegments(this.output);
            return ruleExtended;
        }

        /// <summary>
        /// Returns the OutputSegments. usefull for building a string quickly.
        /// This is one difference between the RimWorld's interaction system and the culture system
        /// </summary>
        public OutputSegment[] Segments
        {
            get
            {
                if (this.segments == null)
                    this.segments = GenerateOutputSegments(this.output);
                return this.segments;
            }
        }

        public RuleExtended() { }

        public RuleExtended(string keyword, string output)
        {
            this.keyword = keyword;
            this.output = output;
        }

        public RuleExtended(string rawText)
        {
            ruleString = new Rule_String(rawText);
            this.output      = ruleString.Generate();
            this.weight      = ruleString.BaseSelectionWeight;
            this.priority    = ruleString.Priority;
            this.keyword     = ruleString.keyword;
            this.tag         = ruleString.tag;
            this.requiredTag = ruleString.requiredTag;
            this.usesLimit   = ruleString.usesLimit;
            this.constantConstraints = ruleString.constantConstraints;
        }


        /// <summary>
        /// never have [] or [something_something_something] in a rule. that will break things.
        /// </summary>
        /// <param name="output"></param>
        /// <returns></returns>
        private OutputSegment[] GenerateOutputSegments(string output)
        {
            string[] segmentStrings = output.Split(new char[] { ']' });
            // most segments will look like this: " text here [PREFIX_value"
            // the last one looks like this: " text here."
            OutputSegment[] segments = new OutputSegment[segmentStrings.Length];

            for (int i = 0; i < -1 + segmentStrings.Length; i++)
            {
                string[] tmp = segmentStrings[i].Split(new char[] { '[' });
                string[] values = tmp[1].Split(new char[] { '_' });
                string seg = tmp[0];
                string PRE = (values.Length == 2) ? values[0] : null;
                string val = (values.Length == 2) ? values[1] : values[0];

                segments[i] = new OutputSegment { text = seg, PREFIX = PRE, value = val };

            } // last element excluded
            // the last segment
            segments[segmentStrings.Length - 1] = new OutputSegment
            { text = segmentStrings[segmentStrings.Length - 1], PREFIX = null, value = null };

            return segments;
        }



        // +------------------------+
        // |     The Variables      |
        // +------------------------+


        // Inherited from Verse.Rule:
        // public string keyword;
        // public string tag;
        // public string requiredTag;
        // public int? usesLimit;
        // public List<Rule.ConstantConstraint> constantConstraints;

        private string output;
        private float weight;
        private float priority;
        private OutputSegment[] segments;

        private Rule_String ruleString;


        /// <summary>
        /// "some text is here [&lt;rulePrefix>_&lt;ruleName>]"
        /// </summary>
        public struct OutputSegment
        {
            public string text;
            public string PREFIX;
            public string value;
        }

    }
}
