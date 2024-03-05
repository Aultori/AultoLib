using AultoLib.Database;
using RimWorld;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.Grammar
{
    public class ExtendedRule_String : ExtendedRule
    {
        public ExtendedRule_String() { }

        public ExtendedRule_String(string key, string value)
        {
            this.keyword = key;
            this.segmentList = new RuleSegments(value);
        }

        public override RuleSegments GetSegments()
        {
            return this.segmentList;
        }

        public override void Initialize(string rawString)
        {
            if (ExtendedRule_String_Loader.TryBuildRule(rawString, out ExtendedRule_String rule))
            {
                this.BecomeCopyOf(rule);
                this.segmentList = rule.segmentList;
            }
            else
                Log.Error($"{Globals.LOG_HEADER} could not build rule from string: {rawString}");
        }

        public new ExtendedRule_String DeepCopy()
        {
            ExtendedRule_String rule = (ExtendedRule_String)base.DeepCopy();
            rule.segmentList = this.segmentList;
            return rule;
        }

        public string ToSimpleString()
        {
            return $"{this.keyword}->{this.segmentList}";
        }

        public RuleSegments segmentList;

    }
}
