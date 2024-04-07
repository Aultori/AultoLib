using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    public class CommunicationCapacities
    {
        public void PostLoad()
        {
            foreach (var capacity in list)
                cachedCapacities.Add(capacity.name, capacity);
        }
        public void ResolveReferences()
        {

        }



        public bool TryGetCapacity(string name, out CommunicationMethod capacity) => cachedCapacities.TryGetValue(name, out capacity);





        // +-----------------+
        // |    Variables    |
        // +-----------------+

        // this goes in SocietyDef?? no
        public List<string> preferred;

        public List<CommunicationMethod> list;

        [Unsaved(false)]
        private Dictionary<string, CommunicationMethod> cachedCapacities = new Dictionary<string, CommunicationMethod>();

    }

    /// <summary>
    /// needed for every communication capacity each race has, because each race has different abilities: hearing, radio recieving, sign language, etc... <br/>
    /// and different ranges for each of them
    /// </summary>
    public class CommunicationMethod
    {
        public string name;
        // public PawnCapacityDef capacityDef; // contains PawnCapacityWorker
        public Type communicationWorker = typeof(CommunicationWorker);
        public SimpleCurve comfortableDistance;
        public SimpleCurve recievingDistance;
    }
}
