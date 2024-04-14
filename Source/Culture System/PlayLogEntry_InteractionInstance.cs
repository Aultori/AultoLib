using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using AultoLib.Database;
using AultoLib.Grammar;
using AultoLib.CustomProperties;

namespace AultoLib
{
    public class PlayLogEntry_InteractionInstance : LogEntry
    {

        public Pawn GetInitiator() => this.initiator;
        public Pawn GetRecipient() => this.recipient;
        public PlayLogEntry_Interaction GetLinkedInteraction() => this.linkedInteraction;

        protected string InitiatorName => this.initiator?.LabelShort ?? "null";
        protected string RecipientName => this.recipient?.LabelShort ?? "null";

        public PlayLogEntry_InteractionInstance() : base(null) { this.linkedInteraction = LinkedProperty_PlayLogEntryInteraction.CreateLink(this); }

        public PlayLogEntry_InteractionInstance(InteractionInstanceDef intInstDef, Pawn initiator, Pawn recipient, List<RulesetDef> extraRulesets = null, HashSet<string> extraTags = null, Constants extraConstants = null)
        {
            this.linkedInteraction = LinkedProperty_PlayLogEntryInteraction.CreateLink(this); // so I can use the interaction bubbles
            this.intInstDef = intInstDef;
            this.initiator = initiator;
            this.recipient = recipient;
            this.initiatorSociety = initiator.Society();
            this.recipientSociety = recipient.Society();
            this.extraRulesets = extraRulesets;
            this.extraTags = extraTags;
            this.extraConstants = extraConstants ?? new Constants();
            this.initiatorFaction = initiator.Faction;
            this.initiatorIdeo = initiator.Ideo;
        }

        public bool FieldsSet()
        {
            return this.intInstDef != null
                && this.initiator != null
                && this.recipient != null
                && this.initiatorSociety != null
                && this.recipientSociety != null
                && this.extraRulesets != null
                && this.initiatorFaction != null
                && this.initiatorIdeo != null;
        }

        public override bool Concerns(Thing t)
        {
            return t == this.initiator || t == this.recipient;
        }

        public override IEnumerable<Thing> GetConcerns()
        {
            if (this.initiator == null) yield return this.initiator;
            if (this.recipient == null) yield return this.recipient;
            yield break;
        }

        public override bool CanBeClickedFromPOV(Thing pov)
        {
            return (pov == this.recipient && CameraJumper.CanJump(this.initiator))
                || (pov == this.initiator && CameraJumper.CanJump(this.recipient));
        }

        public override void ClickedFromPOV(Thing pov)
        {
            if (pov == this.initiator)
            {
                CameraJumper.TryJumpAndSelect(this.recipient, CameraJumper.MovementMode.Pan);
                return;
            }
            if (pov == this.recipient)
            {
                CameraJumper.TryJumpAndSelect(this.initiator, CameraJumper.MovementMode.Pan);
                return;
            }
            throw new NotImplementedException();
        }

        public override Texture2D IconFromPOV(Thing pov)
        {
            return this.intInstDef.GetSymbol(this.initiatorFaction, this.initiatorIdeo);
        }

        public override Color? IconColorFromPOV(Thing pov)
        {
            return this.intInstDef.GetSymbolColor(this.initiatorFaction);
        }

        public override void Notify_FactionRemoved(Faction faction)
        {
            if (this.initiatorFaction == faction) this.initiatorFaction = null;
        }

        public override void Notify_IdeoRemoved(Ideo ideo)
        {
            if (this.initiatorIdeo == ideo) this.initiatorIdeo = null;
        }

        public override string GetTipString()
        {
            // change this?
            return this.intInstDef.LabelCap + "\n" + base.GetTipString();
        }

        // public string ToGameStringFromPOV(Thing pov, bool forceLog) => ToGameStringFromPOV_Worker(pov, forceLog);

        protected override string ToGameStringFromPOV_Worker(Thing pov, bool forceLog)
        {
            if (AultoLog.DoLog()) AultoLog.Message("Going To Resolve Something");
            if (this.initiator == null || this.recipient == null)
            {
                Log.ErrorOnce("PlayLogEntry_InteractionInstance has a null pawn reference.", 34422); // I can use the same key since the default one should never get called
                return $"[{this.intInstDef.label} error: null pawn reference]";
            }

            Rand.PushState();
            Rand.Seed = this.logID;


            try
            {
                if (AultoLog.DoLog()) AultoLog.Message("got here while resolving 1");
                Constants initiatorConstants = ConstantUtil.EnumerableConstantsForPawn("INITIATOR", this.initiator).ToConstants();
                Constants recipientConstants = ConstantUtil.EnumerableConstantsForPawn("RECIPIENT", this.recipient).ToConstants();

                string text;
                ResolverInstance.Reset();
                if (AultoLog.DoLog()) AultoLog.Message("got here while resolving 1.5");
                // an error is thrown here
                ResolverInstance.AddConstants(initiatorConstants);
                ResolverInstance.AddConstants(recipientConstants);
                //ResolverInstance.AddConstants(this.extraConstants);
                if (AultoLog.DoLog()) AultoLog.Message("got here while resolving 2");
                ResolverInstance.AddThingSociety("INITIATOR", this.initiatorSociety.Key);
                ResolverInstance.AddThingSociety("RECIPIENT", this.recipientSociety.Key);
                ResolverInstance.ExtraTags(this.extraTags);

                string communicationPrefix;
                if (!ResolverInstance.AllConstants.TryGetValue("communicationPrefix", out communicationPrefix))
                    communicationPrefix = "";

                if (AultoLog.DoLog()) AultoLog.Message("got here while resolving 3");

                if (pov == this.initiator)
                {
                    // ResolverInstance.ACTIVE_SOCIETY = this.initiatorSociety.KeyUpper;
                    ResolverInstance.SetActiveSociety(this.initiatorSociety.Key);
                    ResolverInstance.AddInstanceRuleset(this.intInstDef.LogRulesInitiator);
                    MacroResolver.TryResolve("r_logentry", "interaction from initiator", out text);
                    // if (this.extraRulesets == null) return text;
                }
                else if (pov == this.recipient)
                {
                    ResolverInstance.SetActiveSociety(this.recipientSociety.Key);
                    ResolverInstance.AddInstanceRuleset(this.intInstDef.LogRulesRecipient ?? this.intInstDef.LogRulesInitiator);
                    MacroResolver.TryResolve("r_logentry", "interaction from recipient", out text);
                    // if (this.extraRulesets == null) return text;
                }
                else
                {
                    Log.ErrorOnce("Cannot display PlayLogEntry_InteractionInstance from POV who isn't initiator or recipient.", 51251); Log.ErrorOnce("Cannot display PlayLogEntry_Interaction from POV who isn't initiator or recipient.", 51251);
                    return null;
                }
                if (this.extraRulesets == null) return communicationPrefix+text;

                foreach (RulesetDef rulesetDef in this.extraRulesets)
                {
                    ResolverInstance.Reset();
                    ResolverInstance.AddInstanceRuleset(rulesetDef.ruleset);
                    ResolverInstance.AddConstants(initiatorConstants);
                    ResolverInstance.AddConstants(recipientConstants);
                    ResolverInstance.AddConstants(extraConstants);
                    ResolverInstance.AddThingSociety("INITIATOR", this.initiatorSociety.Key);
                    ResolverInstance.AddThingSociety("RECIPIENT", this.recipientSociety.Key);
                    ResolverInstance.ExtraTags(this.extraTags);
                    MacroResolver.TryResolve(rulesetDef.FirstKeyword, "extraRulepack", out string text2);
                    text += " " + text2;
                }
                return communicationPrefix+text;
            }
            finally
            {
                Rand.PopState();
            }
        }

        public string Test_ToGameStringFromPOV_Worker(InteractionInstanceDef intInstDef, ConstantUtil.PawnData initiator, ConstantUtil.PawnData recipient)
        {
            Rand.PushState();
            Rand.Seed = this.logID;

            Constants initiatorConstants = ConstantUtil.EnumerableConstantsForPawn("INITIATOR", initiator).ToConstants();
            Constants recipientConstants = ConstantUtil.EnumerableConstantsForPawn("RECIPIENT", recipient).ToConstants();

            ResolverInstance.Reset();
            ResolverInstance.AddConstants(initiatorConstants);
            ResolverInstance.AddConstants(recipientConstants);
            ResolverInstance.AddConstants(extraConstants);
            ResolverInstance.AddThingSociety("INITIATOR", this.initiatorSociety.Key);
            ResolverInstance.AddThingSociety("RECIPIENT", this.recipientSociety.Key);

            try
            {
                ResolverInstance.SetActiveSociety(initiator.society.Key);
                ResolverInstance.AddInstanceRuleset(intInstDef.LogRulesInitiator);
                MacroResolver.TryResolve("r_logentry", "interaction from initiator", out string text);
                return text;
            }
            finally
            {
                Rand.PopState();
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<InteractionInstanceDef>(ref this.intInstDef, "intDef");
            Scribe_References.Look<Pawn>(ref this.initiator, "initiator", true);
            Scribe_References.Look<Pawn>(ref this.recipient, "recipient", true);
            Scribe_Defs.Look<SocietyDef>(ref this.initiatorSociety, "initiatorSociety");
            Scribe_Defs.Look<SocietyDef>(ref this.recipientSociety, "recipientSociety");
            Scribe_Collections.Look<RulesetDef>(ref this.extraRulesets, "extras", LookMode.Undefined, Array.Empty<object>());
            Scribe_References.Look<Faction>(ref this.initiatorFaction, "initiatorFaction", false);
            Scribe_References.Look<Ideo>(ref this.initiatorIdeo, "initiatorIdeo", false);
        }

        public override string ToString()
        {
            return $"{this.intInstDef.label}: {this.InitiatorName}->{this.RecipientName}";
        }

        // +-----------------+
        // |    Variables    |
        // +-----------------+

        protected InteractionInstanceDef intInstDef;

        protected Pawn initiator;
        protected Pawn recipient;
        protected SocietyDef initiatorSociety; 
        protected SocietyDef recipientSociety;

        protected List<RulesetDef> extraRulesets;
        protected HashSet<string> extraTags;
        protected Constants extraConstants;

        public Faction initiatorFaction; 
        public Ideo initiatorIdeo;

        private PlayLogEntry_Interaction linkedInteraction; // doesn't contain anything, but can be used to find this interaction instance
    }
}
