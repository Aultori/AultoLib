using AultoLib.Database;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace AultoLib.Grammar
{
    // should hold all "rules" for a particular keyword, and output a randomly selected possible output, the segments
    /// <summary>
    /// a data structure for rules allowing for quick use by the Resolver.
    /// Generates output segments.
    /// </summary>
    public abstract class ExtendedRule
    {
        // +------------------+
        // |    Properties    |
        // +------------------+
        public float Weight => this.weight;
        public float Priority => this.priority;
        // public string Tag
        // { get { return this.tag; } }
        // public string RequiredTag
        // { get { return this.requiredTag; } }
        public int? UsesLimit => this.usesLimit;
        public RuleConstraints Constraints => this.constraintList;

        public ExtendedRule DeepCopy()
        {
            ExtendedRule rule = (ExtendedRule)Activator.CreateInstance(base.GetType());

            // keyword
            rule.keyword = this.keyword;

            // properties
            rule.weight = this.weight;
            rule.priority = this.priority;
            // rule.tag = this.tag;
            // rule.requiredTag = this.requiredTag;
            rule.usesLimit = this.usesLimit;

            // instance data
            rule.uses = this.uses;
            rule.lastRunID = this.lastRunID;
            rule.knownUnresolveable = this.knownUnresolveable;
            rule.constraintsChecked = this.constraintsChecked;
            rule.constraintsValid = this.constraintsValid;

            // lists
            rule.constraintList = this.constraintList.DeepCopy();

            return rule;
        }

        public void BecomeCopyOf(ExtendedRule rule)
        {

            this.keyword = rule.keyword;

            // properties
            this.weight = rule.weight;
            this.priority = rule.priority;
            // this.tag = rule.tag;
            // this.requiredTag = rule.requiredTag;
            this.usesLimit = rule.usesLimit;

            // instance data
            this.uses = rule.uses;
            this.lastRunID = rule.lastRunID;
            this.knownUnresolveable = rule.knownUnresolveable;
            this.constraintsChecked = rule.constraintsChecked;
            this.constraintsValid = rule.constraintsValid;

            // lists
            this.tags = rule.tags;
            this.savedTags = rule.savedTags;
            this.requiredTags = rule.requiredTags;
            this.constraintList = rule.constraintList;
        }

        /// <summary>
        /// Sets this rule's data.
        /// Only used once during rule loading.
        /// </summary>
        /// <param name="input"></param>
        public abstract void Initialize(string input);

        /// <returns>the segments which will be supbstituted for this rule</returns>
        public abstract RuleSegments GetSegments();



        // +--------------------+
        // |     Rule Stuff     |
        // +--------------------+

        /// <summary>
        /// Resets things to keep stuff from carying over from the last time the resolver ran
        /// </summary>
        /// <returns>true if data had to be reset</returns>
        public bool ProcessInstance(ulong currentRunID)
        {
            if (this.lastRunID == currentRunID)
                return false;
            this.lastRunID = currentRunID;
            this.uses = 0;
            this.knownUnresolveable = false;
            this.constraintsChecked = false;
            this.constraintsValid = false;
            return true;
        }

        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        // Inherited from Verse.Rule, sort of:
        // and RuleEntry

        public string keyword;

        // properties, things like p where "keyword(p=2)->adsfasfasdf"
        public float weight = 1f;
        public float priority = 1f;
        // public string tag;
        // public string requiredTag;
        public int? usesLimit;

        // TODO: add this functionality
        public Dictionary<string, bool> tags; // truthy: add tag. !truthy: remove tag
        public Dictionary<string, bool> savedTags;
        public Dictionary<string, bool> requiredTags; // truthy: needs tag. !truthy: must not have that tag

        public int? uses = 0;
        public ulong lastRunID;
        public bool knownUnresolveable;
        public bool constraintsChecked;
        public bool constraintsValid;

        public RuleConstraints constraintList = new RuleConstraints();


    }
}
