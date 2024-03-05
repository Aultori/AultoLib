using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AultoLib.Grammar;

namespace AultoLib.Database
{
    public static class Constants_Loader
    {
        public static void LoadConstants(Constants constants)
        {
            GrammarDatabase.globalConstants.Clear();
            GrammarDatabase.globalConstants.Add(constants.Enumerator());
        }

    }
}
