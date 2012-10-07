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
        private Dictionary<string, string> keys;
        private SortedList<string, string> keysSorted;
        private bool changed = false;

        public InMemoryStorage()
        {
            this.keys = new Dictionary<string, string>();
        }

        public void Dispose()
        {
            this.keys.Clear();
        }

        /// <summary>
        /// Open the storage for use.
        /// </summary>
        public void Open()
        {            
        }

        /// <summary>
        /// Get the value for the given key from the storage.
        /// </summary>
        /// <param name="key">Key to look up the value of.</param>
        /// <returns>Value stored.</returns>
        public string Get(string key)
        {
            string value;
            this.keys.TryGetValue(key, out value);
            return value;
        }

        /// <summary>
        /// Put the value for the given key in the storage.
        /// </summary>
        /// <param name="key">Key to store value for.</param>
        /// <param name="value">Value to store.</param>
        public void Put(string key, string value)
        {
            this.keys[key] = value;
            changed = true;
        }

        /// <summary>
        /// Delete the key that matches the provided key.
        /// </summary>
        /// <param name="key">Key to delete.</param>
        public void Delete(string key)
        {
            this.keys.Remove(key);
            this.keysSorted.Remove(key);
        }

        /// <summary>
        /// Put the batch of keys and values in the storage.
        /// </summary>
        /// <param name="batch">Batch of keys and values.</param>
        public void PutBatch(Dictionary<string, string> batch)
        {            
            foreach (var column in batch)
            {
                this.Put(column.Key, column.Value);
            }
        }

        /// <summary>
        /// Delete the batch of keys.
        /// </summary>
        /// <param name="batch">Batch of keys to delete.</param>
        public void DeleteBatch(IEnumerable<string> batch)
        {
            foreach (var key in batch)
            {
                this.Delete(key);
            }
        }

        /// <summary>
        /// Get an enumerator to iterate the storage.
        /// </summary>
        /// <returns>Enumerator that allows iterating the storage.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return new SortedList<string, string>(this.keys).GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator to iterate the storage.
        /// </summary>
        /// <returns>Enumerator that allows iterating the storage.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.keys.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator starting from the provided key to iterate the storage.
        /// </summary>
        /// <param name="key">The key to start the enumerator at.</param>
        /// <returns>The enumerator that allows iterating the storage.</returns>
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
