using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace AultoLib.Grammar
{
    /// <summary>
    /// Similar to <c>Verse.Grammar.Rule</c>
    /// The same as<c>Verse.Grammar.Rule</c> just use the original
    /// </summary>
    public abstract class RVC_Rule
    {
        public virtual float Priority 
        {
            get { return 0f; }
        }

        public virtual RVC_Rule DeepCopy()
        {
            RVC_Rule rule = (RVC_Rule)Activator.CreateInstance(base.GetType());
            rule.keyword = this.keyword;
            rule.tag     = this.tag;
            rule.requiredTag = this.requiredTag;
            if (this.constantConstraints != null)
                rule.constantConstraints = this.constantConstraints.ToList<Rule.ConstantConstraint>();
            return rule;
        }

        public abstract string Generate();

        public virtual void Init() { }

        public void AddConstantConstraint(string key, string value, Rule.ConstantConstraint.Type type)
        {
            if (this.constantConstraints == null)
                this.constantConstraints = new List<Rule.ConstantConstraint>();
            this.constantConstraints.Add(new Rule.ConstantConstraint { key = key, value = value, type = type }); 
        }

        public void AddConstantConstraint(string key, string value, string op)
        {
            Rule.ConstantConstraint.Type type;
            switch (op)
            {
                case "==": type = Rule.ConstantConstraint.Type.Equal; break;
                case "!=": type = Rule.ConstantConstraint.Type.NotEqual; break;
                case "<":  type = Rule.ConstantConstraint.Type.Less; break;
                case ">":  type = Rule.ConstantConstraint.Type.Greater; break;
                case "<=": type = Rule.ConstantConstraint.Type.LessOrEqual; break;
                case ">=": type = Rule.ConstantConstraint.Type.GreaterOrEqual; break; 
                default:
                    type = Rule.ConstantConstraint.Type.Equal;
                    Log.Error("Unknown ConstantConstraint type:" + op);
                    break;
            }
            this.AddConstantConstraint(key, value, type);
        }

        public bool ValidateConstraints(Dictionary<string, string> constraints)
        {
            Rule new Rule();
        }

        public string keyword;

        public string tag;

        public string requiredTag;

        public List<Rule.ConstantConstraint> constantConstraints; 

    }
}
