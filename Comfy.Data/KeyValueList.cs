using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.Data
{
    [Serializable]
    public class KeyValueList<TKey, TValue> : List<KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }
    }
}