using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace AultoLib.Grammar
{
    public static class ConstantUtil
    {
        public static IEnumerable<Constants.Constant> EnumerableConstantsForPawn(string pawnSymbol, Pawn pawn, bool addRelationInfoSymbol = true, bool addTags = false)
        {
            if (pawn == null)
            {
                Log.ErrorOnce($"Tried to insert rule {pawnSymbol} for null pawn", 16015097);
                return Enumerable.Empty<Constants.Constant>();
            }
            TaggedString taggedString = "";
            if (addRelationInfoSymbol)
            {
                PawnRelationUtility.TryAppendRelationsWithColonistsInfo(ref taggedString, pawn);
            }
            PawnData pawnData = new PawnData
            {
                  pawn = pawn
                , name = pawn.Name
                , title = pawn.story?.Title
                , kind = pawn.kindDef
                , gender = pawn.gender
                , faction = pawn.Faction
                , age = pawn.ageTracker.AgeBiologicalYears
                , chronologicalAge = pawn.ageTracker.AgeChronologicalYears
                , relationInfo = taggedString
                , everBeenColonistOrTameAnimal = PawnUtility.EverBeenColonistOrTameAnimal(pawn)
                , everBeenQuestLodger = PawnUtility.EverBeenQuestLodger(pawn)
                , isFactionLeader = pawn.Faction != null && pawn.Faction.leader == pawn
                , royalTitles = pawn.royalty?.AllTitlesForReading
            };
            return ConstantUtil.EnumerableConstantsForPawn(pawnSymbol, pawnData, addTags);
        }

        // public string GetFromSelf(string keyword)
        // {
        //     string value;
        //     if (this.list.TryGetValue(keyword, out value))
        //         return value;
        //     if (this.TryGet(keyword, out value)) {
        //         this.list.Add(keyword, value);
        //         return value;
        //     }
        //     return null;
        // }

        // public bool TryGet(string suffix, out string value)
        // {
        //     value = null;
        //     //string prefix = data.pawnSymbol.NullOrEmpty() ? data.pawnSymbol : ;
        //     switch (suffix)
        //     {
        //     case "label":
        //         value = NameTag(GetFromSelf("nameShort"));
        //         break;
        //     case "kindLabel":
        //         value = DATA.kind.label;
        //         if (DATA.gender == Gender.Female) value = DATA.kind.labelFemale ?? value;
        //         if (DATA.gender == Gender.Male) value = DATA.kind.labelMale ?? value;
        //         break;
        //     case "nameFull":
        //         if (DATA.name != null)
        //              value = Find.ActiveLanguageWorker.WithIndefiniteArticle(DATA.name.ToStringFull, DATA.gender, false, false);
        //         else value = Find.ActiveLanguageWorker.WithIndefiniteArticle(GetFromSelf("kindLabel"), this.kindGender, false, false);
        //         value = NameTag(value);
        //         break;
        //     case "nameShort":
        //         value = (DATA.name != null) ? DATA.name.ToStringShort : GetFromSelf("kindLabel");
        //         break;
        //     case "nameShortDef":
        //         if (DATA.name != null)
        //              value = Find.ActiveLanguageWorker.WithDefiniteArticle(DATA.name.ToStringShort, DATA.gender, false, true);
        //         else value = Find.ActiveLanguageWorker.WithDefiniteArticle(GetFromSelf("kindLabel"), this.kindGender, false, false);
        //         break;
        //     case "definate":
        //         value = NameTag(GetFromSelf("nameShortDef"));
        //         break;
        //     case "nameDef":
        //         value = NameTag(GetFromSelf("nameShortDef"));
        //         break;
        //     default:
        //         return false;
        //     }
        //     return value != null;
        //     return false;
        // }



        /// <summary>
        /// Creates constants for a <see cref="Pawn"/>.
        /// </summary>
        /// <param name="pawnSymbol"></param>
        /// <param name="DATA"></param>
        /// <param name="addTags"></param>
        /// <param name="addSkills"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IEnumerable<Constants.Constant> EnumerableConstantsForPawn(string pawnSymbol, PawnData DATA, bool addTags = false, bool addSkills = false)
        {
            SetPrefix(pawnSymbol);
            SetAddTags(addTags);

            // two frequently used variables
            string kindLabel;
            Gender kindGender;

            kindLabel = DATA.kind.label;
            if (DATA.gender == Gender.Female) kindLabel = DATA.kind.labelFemale ?? kindLabel;
            if (DATA.gender == Gender.Male) kindLabel = DATA.kind.labelMale ?? kindLabel;
            kindGender = GrammarResolverSimple.ResolveGender(kindLabel, DATA.gender);

            string nameFull;
            string nameShort;
            string nameShortDefinite;
            string nameShortIndefinite;
            string pronoun;
            string possessive;
            string objective;
            string genderResolved;
            if (DATA.name != null)
            {
                nameFull = Find.ActiveLanguageWorker.WithIndefiniteArticle(DATA.name.ToStringFull, DATA.gender, false, true);
                nameShort = DATA.name.ToStringShort;
                nameShortDefinite = Find.ActiveLanguageWorker.WithDefiniteArticle(nameShort, DATA.gender, false, true);
                nameShortIndefinite = Find.ActiveLanguageWorker.WithIndefiniteArticle(nameShort, DATA.gender, false, true);
                pronoun = DATA.gender.GetPronoun();
                possessive = DATA.gender.GetPossessive();
                objective = DATA.gender.GetObjective();
                genderResolved = DATA.gender.ToString();
            }
            else
            {
                nameFull = Find.ActiveLanguageWorker.WithIndefiniteArticle(kindLabel, kindGender, false, false);
                nameShort = kindLabel;
                nameShortDefinite = Find.ActiveLanguageWorker.WithDefiniteArticle(nameShort, kindGender, false, false);
                nameShortIndefinite = Find.ActiveLanguageWorker.WithIndefiniteArticle(nameShort, kindGender, false, false);
                pronoun = kindGender.GetPronoun();
                possessive = kindGender.GetPossessive();
                objective = kindGender.GetObjective();
                genderResolved = kindGender.ToString();
            }
            yield return MakeConstant("nameFull",   NameTag(nameFull));
            yield return MakeConstant("label",      NameTag(nameShort));
            yield return MakeConstant("definite",   NameTag(nameShortDefinite));
            yield return MakeConstant("nameDef",    NameTag(nameShortDefinite));
            yield return MakeConstant("indefinite", NameTag(nameShortIndefinite));
            yield return MakeConstant("nameIndef",  NameTag(nameShortIndefinite));
            yield return MakeConstant("pronoun",    NameTag(pronoun));
            yield return MakeConstant("possessive", NameTag(possessive));
            yield return MakeConstant("objective",  NameTag(objective));

            // things that are usually "constants" in RimWorld
            yield return MakeConstant("gender", DATA.gender.ToString());
            yield return MakeConstant("genderResolved", genderResolved);
            yield return MakeConstant("age", DATA.age.ToString());
            yield return MakeConstant("chronologicalAge", DATA.chronologicalAge.ToString());
            yield return MakeConstant("ageSpentInStasis", (DATA.chronologicalAge - DATA.age).ToString());
            yield return MakeConstant("formerlyColonist", "False");

            if (addSkills && DATA.pawn != null)
            {
                Pawn_SkillTracker skills = DATA.pawn.skills;

                throw new NotImplementedException();
            }

            if (DATA.everBeenColonistOrTameAnimal) {
                // I don't know what this is used for
                yield return new Constants.Constant { keyword = "formerlyColonistInfo", value = "PawnWasFormerlyColonist".Translate(nameShort) };
                yield return MakeConstant("formerlyColonist", "True");
            }
            if (DATA.everBeenQuestLodger) {
                // I don't know what this is used for
                yield return new Constants.Constant { keyword = "formerlyColonistInfo", value = "PawnWasFormerlyLodger".Translate(nameShort) };
                yield return MakeConstant("formerlyColonist", "True");
            }
            yield return MakeConstant("relationInfo", DATA.relationInfo);
            if (DATA.kind != null) yield return MakeConstant("flesh", DATA.kind.race.race.FleshType.defName);
            yield return MakeConstant("factionLeader", DATA.isFactionLeader ? "True" : "False");

            // stuff turned on by presence of data
            if (DATA.faction != null) {
                yield return MakeConstant("factionName", ConstantUtil.addTags ? DATA.faction.Name.ApplyTag(DATA.faction).Resolve() : DATA.faction.Name);
                yield return MakeConstant("faction", DATA.faction.def.defName);
            }
            if (DATA.kind != null) {
                yield return MakeConstant("kind", GenLabel.BestKindLabel(DATA.kind, kindGender, false));
                yield return MakeConstant("kindPlural", GenLabel.BestKindLabel(DATA.kind, kindGender, true));
            }
            if (DATA.title != null) {
                Gender titleGender = LanguageDatabase.activeLanguage.ResolveGender(DATA.title, null, DATA.gender);
                yield return MakeConstant("title", DATA.title);
                yield return MakeConstant("titleIndef", Find.ActiveLanguageWorker.WithIndefiniteArticle(DATA.title, titleGender));
                yield return MakeConstant("titleDef", Find.ActiveLanguageWorker.WithDefiniteArticle(DATA.title, titleGender));
            }
            if (DATA.royalTitles != null)
            {
                int index = 0;
                RoyalTitle bestTitle = null;
                foreach (RoyalTitle title in from x in DATA.royalTitles orderby x.def.index select x)
                {
                    string titleLabel = title.def.GetLabelFor(DATA.gender);
                    yield return MakeConstant($"royalTitle{index}", titleLabel);
                    yield return MakeConstant($"royalTitle{index}Indef", Find.ActiveLanguageWorker.WithIndefiniteArticle(titleLabel));
                    yield return MakeConstant($"royalTitle{index}Def", Find.ActiveLanguageWorker.WithDefiniteArticle(titleLabel));
                    if (title.faction == DATA.faction)
                    {
                        yield return MakeConstant("royalTitleInCurrentFaction", titleLabel);
                        yield return MakeConstant("royalTitleInCurrentFactionIndef", Find.ActiveLanguageWorker.WithIndefiniteArticle(titleLabel));
                        yield return MakeConstant("royalTitleInCurrentFactionDef", Find.ActiveLanguageWorker.WithDefiniteArticle(titleLabel));
                        yield return MakeConstant("royalInCurrentFaction", "True");
                    }
                    if (bestTitle == null || title.def.favorCost > bestTitle.def.favorCost)
                        bestTitle = title;
                    index++;
                }
                if (bestTitle != null)
                {
                    string titleLabel = bestTitle.def.GetLabelFor(DATA.gender);
                    yield return MakeConstant("bestRoyalTitle", titleLabel);
                    yield return MakeConstant("bestRoyalTitleIndef", Find.ActiveLanguageWorker.WithIndefiniteArticle(titleLabel));
                    yield return MakeConstant("bestRoyalTitleDef", Find.ActiveLanguageWorker.WithDefiniteArticle(titleLabel));
                    yield return MakeConstant("bestRoyalTitleFaction", bestTitle.faction.Name);
                }
            }
            yield break;
        }

        public static IEnumerable<Constants.Constant> ConstantsForDef(string prefix, Def def)
        {
            if (def == null)
            {
                Log.ErrorOnce($"{Globals.LOG_HEADER} Tried to create constant {prefix} for null def", 79641686); // idk where this number came from
                yield break;
            }

            SetPrefix(prefix);
            yield return MakeConstant("label", def.label);

            // if (def is SocietyDef def2)
            // {
            //     yield return MakeConstant("labelPlural", def2.labelPlural);

            // }

            if (def is PawnKindDef)
                yield return MakeConstant("labelPlural", ((PawnKindDef)def).GetLabelPlural());
            else
                yield return MakeConstant("labelPlural", Find.ActiveLanguageWorker.Pluralize(def.label));
            yield return MakeConstant("description", def.description);
            yield return MakeConstant("definite", Find.ActiveLanguageWorker.WithDefiniteArticle(def.label));
            yield return MakeConstant("indefinite", Find.ActiveLanguageWorker.WithIndefiniteArticle(def.label));
            yield return MakeConstant("possessive", "Proits".Translate());
            yield break;
        }

        public static IEnumerable<Constants.Constant> EnumerableConstantsForSocietyDef(SocietyDef societyDef)
        {
            SetPrefix(societyDef.KeyUpper);
            yield return MakeConstant("Key", societyDef.KeyUpper);
            yield break;
        }

        private static void SetPrefix(string prefix)
        {
            ConstantUtil.prefix = !prefix.NullOrEmpty() ? $"{prefix}_" : "";
        }
        private static void SetAddTags(bool addTags)
        {
            ConstantUtil.addTags = addTags;
        }
        private static string NameTag(string name)
        {
            return !ConstantUtil.addTags ? name : name.ApplyTag(TagType.Name, null).Resolve();
        }

        /// <summary>
        /// Creates a <c>Constant</c> with the correct prefix added to the beginning of the keyword.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Constants.Constant MakeConstant(string key, string value)
        {
            return new Constants.Constant { keyword = ConstantUtil.prefix + key, value = value };
        }
        public static Constants ToConstants(this IEnumerable<Constants.Constant> constEnumerable)
        {
            Constants constants = new Constants();
            constants.Add(constEnumerable);
            return constants;
        }


        private static string prefix; // so I don't have to supply the prefix to "MakeConstant" all the time

        private static bool addTags;



        public struct PawnData
        {
            public Pawn pawn;
            public Name name;
            public string title;
            public SocietyDef society;
            public PawnKindDef kind;
            public Gender gender;
            public Faction faction;
            public int age;
            public int chronologicalAge;
            public string relationInfo;
            public bool everBeenColonistOrTameAnimal;
            public bool everBeenQuestLodger;
            public bool isFactionLeader;
            public List<RoyalTitle> royalTitles;
        }
    }
}
