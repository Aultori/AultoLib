using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace AultoLib.Grammar
{
    public class ExtendedRulePackDef : Def
    {
        public List<ExtendedRule> RulesPlusIncludes
        {
            get
            {
                throw new NotImplementedException(); 
            }
        }

        // Token: 0x17000169 RID: 361
        // (get) Token: 0x060007F5 RID: 2037 RVA: 0x0002882C File Offset: 0x00026A2C
        public List<ExtendedRule> UntranslatedRulesPlusIncludes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // Token: 0x1700016A RID: 362
        // (get) Token: 0x060007F6 RID: 2038 RVA: 0x000288AA File Offset: 0x00026AAA
        public List<ExtendedRule> RulesImmediate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // Token: 0x1700016B RID: 363
        // (get) Token: 0x060007F7 RID: 2039 RVA: 0x000288C1 File Offset: 0x00026AC1
        public List<ExtendedRule> UntranslatedRulesImmediate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // Token: 0x1700016C RID: 364
        // (get) Token: 0x060007F8 RID: 2040 RVA: 0x000288D8 File Offset: 0x00026AD8
        public string FirstRuleKeyword
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        // Token: 0x1700016D RID: 365
        // (get) Token: 0x060007F9 RID: 2041 RVA: 0x00028908 File Offset: 0x00026B08
        public string FirstUntranslatedRuleKeyword
        {
            get
            {
                throw new NotImplementedException ();
            }
        }

        // Token: 0x060007FA RID: 2042 RVA: 0x00028936 File Offset: 0x00026B36
        public override IEnumerable<string> ConfigErrors()
        {
            throw new NotImplementedException ();
        }

        // Token: 0x060007FB RID: 2043 RVA: 0x00028946 File Offset: 0x00026B46
        public static ExtendedRulePackDef Named(string defName)
        {
            return DefDatabase<ExtendedRulePackDef>.GetNamed(defName, true);
        }

        public List<ExtendedRulePackDef> include;

        private ExtendedRulePack rulePack;

        public bool directTestable;

        [Unsaved(false)]
        private List<ExtendedRule> cachedRules;

        [Unsaved(false)]
        private List<ExtendedRule> cachedUntranslatedRules;
    }
}
