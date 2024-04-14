using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.CustomProperties
{
    public static class LinkedProperty_PlayLogEntryInteraction
    {
        public static PlayLogEntry_Interaction CreateLink(PlayLogEntry_InteractionInstance intInst)
        {
            PlayLogEntry_Interaction interaction = new PlayLogEntry_Interaction();
            linked_instances.SetValue(interaction, intInst);
            return interaction;
        }


        public static PlayLogEntry_InteractionInstance GetInteractionInstance(PlayLogEntry_Interaction interaction)
        {
            if (linked_instances.TryGetValue(interaction, out var interactionInstance))
            {
                return interactionInstance;
            }
            AultoLog.SimpleErrorOnce($"a {nameof(PlayLogEntry_Interaction)} was not linked to a {nameof(PlayLogEntry_InteractionInstance)}", "InteractionInstanceLinkError");
            return null;
        }


        private static CustomProperty<PlayLogEntry_Interaction, PlayLogEntry_InteractionInstance> linked_instances = new CustomProperty<PlayLogEntry_Interaction, PlayLogEntry_InteractionInstance>();
    }
}
