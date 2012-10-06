using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dazzle.Extensions;

namespace Dazzle.Storage
{
    public class InMemoryStorage : IStorage
    {
        //private SortedList<string, string> keys;
        private Dictionary<string, string> keys;
        private SortedList<string, string> keysSorted;
        private bool changed = false;

        public InMemoryStorage()
        {
            //this.keys = new SortedList<string, string>();
            this.keys = new Dictionary<string, string>();
        }

        public void Dispose()
        {
            this.keys.Clear();
        }
        
        public void Open()
        {            
        }

        public string Get(string key)
        {
            string value;
            this.keys.TryGetValue(key, out value);
            return value;
        }

        public void Put(string key, string value)
        {
            this.keys[key] = value;
            changed = true;
        }

        public void PutBatch(Dictionary<string, string> batch)
        {            
            foreach (var column in batch)
            {
                this.Put(column.Key, column.Value);
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new SortedList<string, string>(this.keys).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.keys.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator(string key)
        {
            if (changed)
            {
                this.keysSorted = new SortedList<string, string>(this.keys);
                changed = false;
            }
            return this.keysSorted.GetEnumerator(key);
        }
    }
}
