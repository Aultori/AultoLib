using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// for use instead of InteractionDef
    /// </summary>
    public class CulturalInteractionDef : Def
    {
        public CulturalInteractionWorker Worker
        {
            get
            {
                if (this.workerInt  == null)
                {
                    this.workerInt = (CulturalInteractionWorker)Activator.CreateInstance(this.workerClass);
                    this.workerInt.interaction = this;
                }
                return this.workerInt;
            }
        }
        
        public InteractionWorker OldWorker
        {
            get { return rimWorldInteractionDef.Worker; }
        }

        private Texture2D Symbol
        {
            get
            {
                if (this.symbolTex == null)
                {
                    if (this.rimWorldInteractionDef != null)
                        Log.Error($"{Globals.LOG_HEADER} shouldn't get here asdfasdhfasdfkskdf");
                    else 
                        this.symbolTex = ContentFinder<Texture2D>.Get(this.symbol, true);
                }
                return this.symbolTex;
            }
        }

        public Texture2D GetSymbol(Faction initiatorFaction = null, Ideo initatorIdeo = null)
        {
            if (this.rimWorldInteractionDef != null)
            {
                return this.rimWorldInteractionDef.GetSymbol(initiatorFaction, initatorIdeo);
            }
            InteractionSymbolSource interactionSymbolSource = this.symbolSource;
            if (interactionSymbolSource != InteractionSymbolSource.InitiatorIdeo)
            {
                if (interactionSymbolSource != InteractionSymbolSource.InitiatorFaction)
                    return this.Symbol;
                if (initiatorFaction == null)
                    return null;
                return initiatorFaction.def.FactionIcon;
            }
            else
            {
                if (Find.IdeoManager.classicMode)
                    return this.Symbol;
                if (initatorIdeo == null)
                    return null;
                return initatorIdeo.Icon;
            }
        }

        public Color? GetSymbolColor(Faction initiatorFaction = null)
        {
            if (this.rimWorldInteractionDef != null)
            {
                return this.rimWorldInteractionDef.GetSymbolColor(initiatorFaction);
            }
            if (initiatorFaction != null && this.symbolSource == InteractionSymbolSource.InitiatorFaction)
            {
                return new Color?(initiatorFaction.Color);
            }
            return null;
        }

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            if (this.interactionMote == null)
            {
                this.interactionMote = ThingDefOf.Mote_Speech;
            }
        }

        // +------------------------+
        // |     The Variables      |
        // +------------------------+
        private InteractionDef rimWorldInteractionDef = null;

        private Type workerClass = typeof(InteractionWorker);

        public ThingDef interactionMote;

        public float socialFightBaseChance;

        public ThoughtDef initiatorThought;
        public SkillDef initiatorXpGainSkill;
        public int initiatorXpGainAmount;
        public ThoughtDef recipientThought;
        public SkillDef recipientSpGainSkill;
        public int recipientXpGainAmount;

        public bool ignoreTimeSinceLastInteraction;

        [NoTranslate]
        private string symbol;

        public InteractionSymbolSource symbolSource;

        // I created this class just so I could have these here.
        public AultoLib.Grammar.AultoRulePack logRulesInitiator;
        public AultoLib.Grammar.AultoRulePack logRulesRecipiant;

        [Unsaved(false)] private InteractionWorker OldWorkerInt;
        [Unsaved(false)] private CulturalInteractionWorker workerInt;
        [Unsaved(false)] private Texture2D symbolTex;
    }
}
