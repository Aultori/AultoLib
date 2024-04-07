using System.Runtime.CompilerServices;

namespace AultoLib.CustomProperties
{
    /// <summary>
    /// This can be initialized in a static class somewhere.
    /// then extention methods can be used on a Def to make it look similar to a property.
    /// </summary>
    public class CustomProperty<T,V>
    {
        /// <summary>
        /// attempt to get the value <para/>
        /// <b>Tips</b><br/>
        /// Use this to test if an object has the property.
        /// <br/> If it doesn't, you can add it via <see cref="SetValue(T, V)"/>
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(T obj, out V value)
        {
            if (collection.TryGetValue(obj, out dynamic a))
            {
                if(a is V val)
                {
                    value = val;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public void SetValue(T obj, V value)
        {
            collection.Add(obj, value);
        }

        private readonly ConditionalWeakTable<object, dynamic> collection = new ConditionalWeakTable<object, dynamic>();
    }
}
