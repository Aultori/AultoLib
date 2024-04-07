using System;
using System.Collections.Generic;


namespace AultoLib
{
    public class CaselessDictionary<TKey, TValue> : Dictionary<string, TValue>
    {
        public CaselessDictionary() : base(0, StringComparer.OrdinalIgnoreCase) { }
        public CaselessDictionary(int capacity) : base(capacity, StringComparer.OrdinalIgnoreCase) { }
        public CaselessDictionary(IDictionary<string, TValue> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase) { }
    }
}
