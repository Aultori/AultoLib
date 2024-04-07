using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AultoLib.Grammar
{
    public class RuleSegments
    {
        public RuleSegments()
        {
            this.List = new List<Segment> ();
        }

        /// <summary>
        /// for creating a simple list of segments where there's only one word and no macros.
        /// </summary>
        /// <param name="word"></param>
        public RuleSegments(string word)
        {
            this.List = new List<Segment>
            {
                new Segment { text = word, isMacro = false }
            };
        }

        public ExtendedRule ToExtendedRule()
        {
            ExtendedRule_String rule = new ExtendedRule_String();
            rule.segmentList = this;
            return rule;
        }

        public int Count
        {
            get { return this.List.Count; }
        }

        public void Add(string text)
        {
            this.List.Add(new Segment { text = text, isMacro = false });
        }

        public void Add(string text, RuleMacro macro)
        {
            this.List.Add(new Segment { text = text, macro = macro, isMacro = true });
        }

        public void Add(Segment segment)
        {
            this.List.Add(segment);
        }

        public RuleSegments DeepCopy()
        {
            RuleSegments segments = new RuleSegments();
            foreach (Segment segment in this.List)
                segments.List.Add(segment);
            return segments;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Segment segment in this.List)
                sb.Append(segment.ToString());

            return sb.ToString();
        }
        

        public List<Segment> List;

        public struct Segment
        {
            public override string ToString() => (isMacro) ? $"{text}{macro}" : text ;
            
            public string text;
            public RuleMacro macro;
            public bool isMacro;
        }
    }
}
