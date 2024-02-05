using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AultoLib.Database
{
    public static class CultureDefLoader
    {
        public static void Load(CultureDef culture)
        {
            
        }

        private static void LoadToDatabase(CultureDef culture)
        {
            GrammarDatabase.loadedCultureDefs[culture.PREFIX] = culture;
        }
    }
}
