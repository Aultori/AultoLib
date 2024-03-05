using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// uses InteractionDefs.
    /// Determines the culture to use for replacement text.
    /// Determines which InteractionDef to use based on the recipiant and initiator's culture.
    /// I think Pawns of different cultures can have different reactions to the initiator's interaction.
    ///
    /// </summary>
    ///
    /// Implimentation Explanation
    /// ==========================
    /// Instead of there being one InteractionDef(the data) per
    /// InteractionDef(the interaction) like in RimWorld, I now have
    /// multiple sets of data for a single kind of interaction.
    /// This is because with my Society System, pawns will interact differently depending on the cultures of the initiator and recipient. I needed to have some way of selecting the correct data depending on the situation.
    ///
    /// I can't have this Def work like a category, defineing all the InteractionDefs possible, because it'd make it difficult for mods to add more cultures.
    /// Instead, this Def lists the category it belongs to.
    /// 
    /// When choosing an interacion, I'll look for a list containing ...
    /// The interaction will be chosen from a set of Interaction...
    /// The correct InteractionDef will be chosen 
    /// 
    /// This Def contains the data for a single initiator and recipient combination.
    ///
    /// if the initiatorCulture is "any", it means any culture is valid, not just the fallback.
    /// if the initiatorCulture is "fallback", it means only fallback culture pawns can use it.
    /// 
    /// // planned for later:
    /// // Possible feature: because I'm designing this system, I could make it so a pawn's stats can influence the reaction to an interaction.
    /// // For instance, if a vilos talks to a human about a sientific concept, the inteligence stat of the human can determine whether they understand the topic.
    /// //
    /// // I could nest li elements like if else statements and use parameters to define a simple conditional, then generate a 'stat checker' object which I can later plug the pawns into and evaluate.
    /// // I'll have regular statements and conditional ones. 
    /// // With this functionality, I'd be able to do the thing I described in the paragraph above.
    /// //
    /// // a statement is basically (replacement name-->stuff to expand to) or (replacement name-->conditional to evaluate then expand to)
    /// //
    /// // Because of this complicated thing I'm doing, I'll need to generate a database from the data that my string builder will use.
    /// 
    /// gives categories that these interactions occurr in.
    /// when choosing an interaction, I will look for the list 
    public class CultureInteractionDef : InteractionDef
    {
        // I can't implement this the way i want because I'm unable to override all the InteractionDef methods, so I can't pass this around like an InteractionDef and have things fallback to the refference InteractionDef.
        // I would like to

        public string category;

        public SocietyDef initiatorCulture;

        public SocietyDef recepientCulture;

        /// <summary>
        /// 
        /// </summary>
        public InteractionDef replacementInteractionDef;
    }
}
