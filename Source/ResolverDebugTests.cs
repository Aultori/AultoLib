using System.Collections.Generic;
using System.Text;
using RimWorld;
using Verse;
using AultoLib;
using AultoLib.Grammar;
using AultoLib.Database;
using System.Linq;

namespace RimVilos
{
    public static class ResolverDebugTests
    {
        // my resolver:
        // it needs...
        // things loaded into GrammarDatabase
        // a selected InteractionInstanceDef
        // two pawns, or some test values
        // data about cultures, the CultureDefs for the initiator and recipiant

        // [DebugOutput("RimVilos")]
        // public static void BasicMacroResolverTest()
        // {
        //     List<Pawn> collection = this.pawn.Map.mapPawns.SpawnedPawnsInFaction(this.pawn.Faction);
        // }

        [DebugOutput("RimVilos: Text generation", false)]
        public static void InteractionInstanceLogTest()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            using (List<InteractionInstanceDef>.Enumerator enumerator = DefDatabase<InteractionInstanceDef>.AllDefsListForReading.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    InteractionInstanceDef def = enumerator.Current;
                    list.Add(new DebugMenuOption(def.defName, DebugMenuOptionMode.Action, delegate ()
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        Pawn pawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, null);
                        Pawn recipient = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, null);
                        Log.Message($"{Globals.DEBUG_LOG_HEADER} test of {def.defName}");
                        for (int i = 0; i < 100; i++)
                        {
                            PlayLogEntry_InteractionInstance playLogEntry_Interaction = new PlayLogEntry_InteractionInstance(def, pawn, recipient);
                            stringBuilder.AppendLine(playLogEntry_Interaction.ToGameStringFromPOV(pawn, false));
                        }
                        Log.Message(stringBuilder.ToString());
                    }));
                }
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        [DebugOutput("RimVilos: Text generation", false)]
        public static void CategoryLog()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            using (List<string>.Enumerator enumerator = GrammarDatabase.loadedInteractionInstances.Keys.ToList().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    string category = enumerator.Current;
                    list.Add(new DebugMenuOption(category, DebugMenuOptionMode.Action, delegate ()
                    {
                        Log.Message($"{Globals.DEBUG_LOG_HEADER} test of category: {category}");
                        StringBuilder stringBuilder = new StringBuilder();
                        List<PawnKindDef> pawnKinds = new List<PawnKindDef> { PawnKindDefOf.Colonist, PawnKindDef.Named("RimVilos_Colonist") };

                        // foreach (string kind1 in SocietyDatabase.societyDefs.Keys)
                        foreach (PawnKindDef kind1 in pawnKinds)
                        {
                            foreach (PawnKindDef kind2 in pawnKinds)
                            {
                                Pawn initiator = PawnGenerator.GeneratePawn(kind1, null);
                                Pawn recipient = PawnGenerator.GeneratePawn(kind2, null);

                                TestCategory(stringBuilder, category, initiator, recipient);
                            }
                        }

                        Log.Message(stringBuilder.ToString());
                    }));
                }
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }


        private static void TestCategory(StringBuilder stringBuilder, string category, Pawn initiator, Pawn recipient)
        {
            // get the InteractionInstanceDef
            string initiatorSociety = initiator.Society().KeyUpper;
            string recipientSociety = recipient.Society().KeyUpper;
            stringBuilder.AppendLine("---------------------------------------");

            InteractionInstanceDef inter;
            if (GrammarDatabase.TryGetInteractionInstance(category, initiatorSociety, recipientSociety, out inter))
            {
                stringBuilder.AppendLine($"selected InteractionInstanceDef: {inter.defName}");
                stringBuilder.AppendLine($"initiator: {initiatorSociety}");
                stringBuilder.AppendLine($"recipient: {recipientSociety}");
                stringBuilder.AppendLine("");
                for (int i = 0; i < 16; i++)
                {
                    PlayLogEntry_InteractionInstance playLogEntry_Interaction = new PlayLogEntry_InteractionInstance(inter, initiator, recipient);
                    stringBuilder.AppendLine(playLogEntry_Interaction.ToGameStringFromPOV(initiator, false));
                }
            }
            else
            {
                stringBuilder.AppendLine($"Error, interaction instance not found for {initiatorSociety} {recipientSociety}");
            }
        }


        [DebugOutput("RimVilos: Text generation", false)]
        public static void InteractionInstanceGenerationTest()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            using (List<InteractionInstanceDef>.Enumerator enumerator = DefDatabase<InteractionInstanceDef>.AllDefsListForReading.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    InteractionInstanceDef def = enumerator.Current;
                    list.Add(new DebugMenuOption(def.defName, DebugMenuOptionMode.Action, delegate ()
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (string str in InteractionInstanceStats(def)) stringBuilder.AppendLine(str);

                        Log.Message(stringBuilder.ToString());
                    }));
                }
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }


        public static IEnumerable<string> InteractionInstanceStats(InteractionInstanceDef interactionInstanceDef)
        {
            var def = interactionInstanceDef;

            yield return "== interaction instance stuff ==";
            yield return $"catagory: {def.category}";
            yield return $"initiatorSociety: {def.initiatorSociety}";
            yield return $"recipientSociety: {def.recipientSociety}";
            yield return $"activeSociety: {def.activeSociety}";
            yield return $"communication methods: I haven't written this part yet";
            yield return "";

            yield return "== thoughts and skills ==";
            // if (def.\1 != null) yield return \$"\1: {def.\1.defName}";
            if (def.initiatorThought != null)      yield return $"initiatorThought:      {def.initiatorThought.defName}";
            if (def.initiatorXpGainSkill != null)  yield return $"initiatorXpGainSkill:  {def.initiatorXpGainSkill.defName}";
            if (def.initiatorXpGainAmount != 0)    yield return $"initiatorXpGainAmount: {def.initiatorXpGainAmount}";
            if (def.recipientThought != null)      yield return $"recipientThought:      {def.recipientThought.defName}";
            if (def.recipientSpGainSkill != null)  yield return $"recipientSpGainSkill:  {def.recipientSpGainSkill.defName}";
            if (def.recipientXpGainAmount != 0)    yield return $"recipientXpGainAmount: {def.recipientXpGainAmount}";
            yield return "";

            
            if (def.LogRulesInitiator != null)
            {
                yield return "== LogRulesInitiator ==";
                foreach (string str in RulesetDescription(def.LogRulesInitiator)) yield return "    " + str;
                yield return "";
            }
            if (def.LogRulesRecipient != null)
            {
                yield return "== LogRulesRecipient ==";
                foreach (string str in RulesetDescription(def.LogRulesRecipient)) yield return "    " + str;
                yield return "";
            }

            yield break;
        }

        public static IEnumerable<string> RulesetDescription(Ruleset ruleset)
        {
            Dictionary<string, CompoundRule> keyValues = ruleset.RulesPlusDefs;

            yield return "== CompoundRule(s) ==";
            yield return $"count: {keyValues.Count}";
            foreach (string key in keyValues.Keys) yield return $"{key} count: {keyValues[key].RuleCount}";
            yield return "";

            yield return "== ExtendedRule_String ==";
            foreach (string key in keyValues.Keys)
            {
                foreach (ExtendedRule rule in keyValues[key].rules)
                {
                    if (rule is ExtendedRule_String rule2) yield return rule2.ToSimpleString();
                }
            } 


            yield break;
        }

    }
}
