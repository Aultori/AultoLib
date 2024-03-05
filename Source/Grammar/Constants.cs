using System.Collections.Generic;
using Verse;

namespace AultoLib.Grammar
{
    public class Constants
    {

        public Constants()
        {
            this.list = new Dictionary<string, string>();
        }

        /// <summary>
        /// A dictionary containing the constants
        /// </summary>
        public Dictionary<string,string> List { get { return this.list; } }
        
        public bool TryGetValue(string keyword, out string value)
        {
            return this.list.TryGetValue(keyword, out value);
        }

        public IEnumerable<Constants.Constant> Enumerator()
        {
            return (IEnumerable<Constants.Constant>)this.list;
        }

        public void Clear()
        {
            this.list.Clear();
        }

        public void Add(Constants constants)
        {
            this.list.AddRange(constants.list);
        }
        
        public void Add(IEnumerable<Constants.Constant> constants)
        {
            foreach (var constant in constants) this.Add(constant);
        }

        public void Add(Constants.Constant constant)
        {
            this.list[constant.keyword] = constant.value;
        }


        private Dictionary<string, string> list;

        public struct Constant
        {
            public string keyword;
            public string value;
        }
    }
}
