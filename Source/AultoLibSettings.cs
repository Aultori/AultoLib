using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace AultoLib
{
    public class AultoLibSettings : ModSettings
    {

        public bool interactions;
        public bool doInteractionHeader = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref interactions, "interactions", true);
            Scribe_Values.Look(ref doInteractionHeader, "interaction header", true);
            
        }
    }
}
