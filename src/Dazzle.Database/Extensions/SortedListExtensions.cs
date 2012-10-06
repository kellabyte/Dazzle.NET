using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dazzle.Extensions
{
    public static class SortedListExtensions
    {
        public static IEnumerator<KeyValuePair<K, V>> GetEnumerator<K, V>(this SortedList<K, V> list, K key)
        {
            var keys = list.Keys.ToList();
            var values = list.Values.ToList();
            for (int i = list.IndexOfKey(key); i < list.Count; i++)
            {
                int test = i;
                yield return new KeyValuePair<K, V>(keys[i], values[i]);
            }
        }
    }
}
