namespace AultoLib
{
    using RimWorld;
    using Verse;


    [DefOf]
    public static class SocietyDefOf
    {
        static SocietyDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SocietyDefOf));
        }

        public static SocietyDef fallback;
    }


    [DefOf]
    public class RulesetDefOf
    {
        static RulesetDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SocietyDefOf));
        }

        public static RulesetDef GlobalUtility_Fallback;
        public static RulesetDef TalkTopicsUtility_Fallback;
        public static RulesetDef Sentence_SocialFightStarted;
    }

    [DefOf]
    public static class CategoryDefOf
    {
        public static InteractionCategoryDef Insult;
    }


    [DefOf]
    public static class PawnCapacityDefOf
    {
        public static PawnCapacityDef SoundPlayback;
        public static PawnCapacityDef Scanning;
        public static PawnCapacityDef EMReceiving;
        public static PawnCapacityDef EMBroadcasting;
        public static PawnCapacityDef Magnetoreception;
        public static PawnCapacityDef Magnetism;
    }

    [DefOf]
    public static class BodyPartTagDefOf
    {
        public static BodyPartTagDef ScanningSource;
        public static BodyPartTagDef EMReceivingSource;
        public static BodyPartTagDef EMBroadcastingSource;
        public static BodyPartTagDef MagnetoreceptionSource;
    }
}
