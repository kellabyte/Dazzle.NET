using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dazzle.Storage
{
    public interface IStorage : IEnumerable<KeyValuePair<string, string>>, IDisposable
    {
        /// <summary>
        /// Open the storage for use.
        /// </summary>
        void Open();

        /// <summary>
        /// Get the value for the given key from the storage.
        /// </summary>
        /// <param name="key">Key to look up the value of.</param>
        /// <returns>Value stored.</returns>
        string Get(string key);

        /// <summary>
        /// Put the value for the given key in the storage.
        /// </summary>
        /// <param name="key">Key to store value for.</param>
        /// <param name="value">Value to store.</param>
        void Put(string key, string value);

        /// <summary>
        /// Delete the key that matches the provided key.
        /// </summary>
        /// <param name="key">Key to delete.</param>
        void Delete(string key);

        /// <summary>
        /// Put the batch of keys and values in the storage.
        /// </summary>
        /// <param name="batch">Batch of keys and values.</param>
        void PutBatch(Dictionary<string, string> batch);

        /// <summary>
        /// Delete the batch of keys.
        /// </summary>
        /// <param name="batch">Batch of keys to delete.</param>
        void DeleteBatch(IEnumerable<string> batch);

        /// <summary>
        /// Get an enumerator to iterate the storage.
        /// </summary>
        /// <returns>Enumerator that allows iterating the storage.</returns>
        IEnumerator<KeyValuePair<string, string>> GetEnumerator();

        /// <summary>
        /// Get an enumerator starting from the provided key to iterate the storage.
        /// </summary>
        /// <param name="key">The key to start the enumerator at.</param>
        /// <returns>The enumerator that allows iterating the storage.</returns>
        IEnumerator<KeyValuePair<string, string>> GetEnumerator(string key);
    }
}
