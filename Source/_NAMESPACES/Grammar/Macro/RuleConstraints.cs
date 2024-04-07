using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using Verse;

namespace AultoLib.Grammar
{
    /// <summary>
    /// Meant for use by rule strings, but can be used by any of them
    /// </summary>
    public class RuleConstraints
    {

        public RuleConstraints DeepCopy()
        {
            RuleConstraints constraints = new RuleConstraints();
            foreach (Constraint element in this.constraintList)
                constraints.AddConstraint(element);
            return constraints;
        }

        public void AddConstraint(Constraint constraint) => this.constraintList.Add(constraint);
        public void AddConstraint(string key, string value, Constraint.Relation relation)
        {
            this.constraintList.Add(new Constraint { key = key, value = value, relation = relation });
        }
        public void AddConstraint(string key, string op, string value)
        {
            Constraint.Relation relation;
            switch (op)
            {
                case "==": relation = Constraint.Relation.EQ; break;
                case "!=": relation = Constraint.Relation.NE; break;
                case "<":  relation = Constraint.Relation.LT; break;
                case ">":  relation = Constraint.Relation.GT; break;
                case "<=": relation = Constraint.Relation.LE; break;
                case ">=": relation = Constraint.Relation.GE; break;
                default:
                    relation = Constraint.Relation.EQ;
                    Log.Error($"{Globals.LOG_HEADER} Unknown Constraint relation: {op}");
                    break;
            }
            this.AddConstraint(key, value, relation);
        }

        public bool Test(Constants constants)
        {
            return this.Test(constants.List);
        }

        public bool Test(Dictionary<string, string> constants)
        {
            if (this.constraintList == null)
                return true;
            
            bool success = true;

            foreach (Constraint constraint in this.constraintList)
            {
                if (!success)
                    break;

                if (!constants.TryGetValue(constraint.key, out string text))
                {
                    Log.Error($"{Globals.LOG_HEADER} Constraint '{constraint.key}' not found.");
                    return false;
                }
                bool isNumber;
                isNumber = float.TryParse(text, out float a);
                isNumber &= float.TryParse(constraint.value, out float b);
                switch (constraint.relation)
                {
                    case Constraint.Relation.EQ: success = text.EqualsIgnoreCase(constraint.value); break;
                    case Constraint.Relation.NE: success = !text.EqualsIgnoreCase(constraint.value); break;
                    case Constraint.Relation.LT: success = (isNumber && a < b); break;
                    case Constraint.Relation.GT: success = (isNumber && a > b); break;
                    case Constraint.Relation.LE: success = (isNumber && a <= b); break;
                    case Constraint.Relation.GE: success = (isNumber && a >= b); break;
                }
            }

            return success;
        }

        public List<Constraint> Constraints
        {
            get { return this.constraintList; }
        }

        public List<Constraint> constraintList = new List<Constraint>();

        public struct Constraint
        {
            public string key;
            public string value;

            /// <summary>
            /// is <c>key</c> relation <c>value</c>
            /// where relation is some comparison
            /// </summary>
            public Relation relation;

            public enum Relation
            {
                EQ, // equal
                NE, // not equal
                LT, // less than
                GT, // greater than
                LE, // less than or equal
                GE, // greater than or equal
            }
        }
    }
}
