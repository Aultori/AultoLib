using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// A Race's ability to recieve data from communications. 
    /// </summary>
    public class CommunicationRecieverDef : Def
    {

        public CommunicationRecieverWorker RecieverWorker
        {
            get
            {
                if (this.workerInt == null)
                {
                    this.workerInt = (CommunicationRecieverWorker)Activator.CreateInstance(this.receiverWorkerClass);
                }
                return this.workerInt;
            }
        }

        public CommunicationMediumDef medium; // something like auditory or sound

        // public PawnCapacityDef capacity;

        public Type receiverWorkerClass = typeof(CommunicationRecieverWorker);
        // handle loudness and reception stuff later.

        [Unsaved(false)]
        private CommunicationRecieverWorker workerInt;
    }
}
