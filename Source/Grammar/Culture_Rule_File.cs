using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.Grammar;

namespace AultoLib
{
    // Note: when building the strings, run it through my thing first, then run it through rimworld's default resolver
    /// <summary>
    /// similar to <c>Verse.Grammar.Rule_File</c>
    /// </summary>
    public class Culture_Rule_File : Rule
    {
        public override float BaseSelectionWeight => throw new NotImplementedException();

        public override Rule DeepCopy()
        {
            Culture_Rule_File culture_Rule_File = (Culture_Rule_File)base.DeepCopy();
            culture_Rule_File.path = this.path;
            if (this.pathList != null)
                culture_Rule_File.pathList = this.pathList.ToList<string>();
            if (this.stringListList != null)
                culture_Rule_File.stringListList = this.stringListList.ConvertAll(stringList => stringList.ToList());
            return culture_Rule_File; 
        }

        public override string Generate()
        {
            throw new NotImplementedException();
        }

        public override void Init()
        {
            if (!this.path.NullOrEmpty())
            {
                this.LoadStringsFromFile(this.path);
            }
            foreach (string path in this.pathList)
            {
                this.LoadStringsFromFile(path);
            }
        }

        private void LoadStringsFromFile(string path)
        {
            List<List<string>> list;
            if (FileUtil.TryGetStringListListForFile(path, this.culture, out list))
            {
                foreach (List<string> stringList in list)
                {
                    this.stringListList.Add(stringList);
                }
            }
        }

        public CultureDef culture;

        public string path;

        public List<string> pathList = new List<string>();

        [Unsaved(false)]
        private List<List<string>> stringListList = new List<List<string>>();
    }
}
