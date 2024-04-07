using System;
using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using AultoLib.Database;

namespace AultoLib
{
    /// <summary>
    /// for use instead of InteractionDef
    /// </summary>
    public class InteractionInstanceDef : Def
    {
        public InteractionInstanceWorker Worker
        {
            get
            {
                if (this.workerInt == null)
                {
                    this.workerInt = (InteractionInstanceWorker)Activator.CreateInstance(this.workerClass);
                    this.workerInt.interaction = this;
                }
                return this.workerInt;
            }
        }
        public AultoLib.Grammar.Ruleset LogRulesInitiator => (this.interactionInstanceDef ?? this).logRulesInitiator;

        public AultoLib.Grammar.Ruleset LogRulesRecipient => (this.interactionInstanceDef ?? this).logRulesRecipient;


        private Texture2D Symbol
        {
            get
            {
                return (this.symbolTex) ?? (this.symbolTex = ContentFinder<Texture2D>.Get(this.symbol, true));
            }
        }

        public Texture2D GetSymbol(Faction initiatorFaction = null, Ideo initatorIdeo = null)
        {
            InteractionSymbolSource interactionSymbolSource = this.symbolSource;
            if (interactionSymbolSource != InteractionSymbolSource.InitiatorIdeo)
            {
                if (interactionSymbolSource != InteractionSymbolSource.InitiatorFaction) return this.Symbol;
                return initiatorFaction?.def.FactionIcon;
            }
            else
            {
                if (Find.IdeoManager.classicMode) return this.Symbol;
                return initatorIdeo?.Icon;
            }
        }

        public Color? GetSymbolColor(Faction initiatorFaction = null)
        {
            if (initiatorFaction != null && this.symbolSource == InteractionSymbolSource.InitiatorFaction)
            {
                return new Color?(initiatorFaction.Color);
            }
            return null;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (this.interactionMote == null) this.interactionMote = ThingDefOf.Mote_Speech;
            InteractionInstanceDef_Loader.Load(this);

            Logging.Message($"loaded {Logging.ColoredDefInformation(this)} to the database");
        }

        // public override void PostLoad()
        // {
        // }


        // +----------------------------------+
        // |    Interaction Instance stuff    |
        // +----------------------------------+

        /// <summary>
        /// should be the label of the InteractionDef?
        /// the kind of interation, the group.
        /// not an individual outcome, but the name of the set of outcomes
        /// </summary>
        public string category;

        // these should actually be strings because some mods might have extra interactions for cultures the user doesn't have the mod for.
        // If that culture doesn't exist, just ignore it.
        /// <summary>
        /// A string representing the society. all lowercase. can be "any", in which case it will be looked for last.
        /// </summary>
        public string initiatorSociety; // self explanitory
        public string recipientSociety; // self explanitory

        public string activeSociety;

        // /// <summary>
        // /// Communication method classes. Talking, body language, things like that.
        // /// The first one in the list will be checked for first.
        // /// </summary>
        // // I don't need to worry about this right now, since I'm just trying to get the sentence generator working.
        // public List<CommunicationMethod> communicationMethods;
        // I don't need this at all?



        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        // private Type workerClass = typeof(InteractionWorker);
        protected readonly Type workerClass = typeof(InteractionInstanceWorker);


        public float socialFightBaseChance; // do I want to change how social fights work?

        public ThoughtDef initiatorThought;
        public SkillDef   initiatorXpGainSkill;
        public int        initiatorXpGainAmount;
        public ThoughtDef recipientThought;
        public SkillDef   recipientXpGainSkill;
        public int        recipientXpGainAmount; 

        public bool ignoreTimeSinceLastInteraction;

        [NoTranslate]
        private string symbol;

        /// <summary>
        /// An optional InteractionInstanceDef to load the rules from.
        /// Helpful if you want to reuse rules, but want different stats and cultures.
        /// </summary>
        protected InteractionInstanceDef interactionInstanceDef;

        public InteractionSymbolSource symbolSource;
        public ThingDef interactionMote;

        // I created this class just so I could have these here.
        protected AultoLib.Grammar.Ruleset logRulesInitiator;
        protected AultoLib.Grammar.Ruleset logRulesRecipient;

        [Unsaved(false)] private InteractionInstanceWorker workerInt;
        [Unsaved(false)] private Texture2D symbolTex;

        [Unsaved(false)] public bool isLoaded = false;
    }
}
