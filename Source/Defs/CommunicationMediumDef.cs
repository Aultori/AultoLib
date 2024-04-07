using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib
{
    /// <summary>
    /// The medium through which a communication/interaction utilizes to carry information, like air or radio waves, or even sign language
    /// </summary>
    public class CommunicationMediumDef : Def
    {
        public CommunicationMediumWorker Worker => this.workerInt ?? (this.workerInt = (CommunicationMediumWorker)Activator.CreateInstance(this.workerClass));

        public Type workerClass = typeof(CommunicationMediumWorker);

        [Unsaved(false)]
        private CommunicationMediumWorker workerInt;
    }
}
