using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LevelDB;

namespace Dazzle.Storage
{
    public class LevelDBStorage : IStorage
    {
        private string path;
        private DB db;

        public LevelDBStorage(string path)
        {
            this.path = path;
        }

        /// <summary>
        /// Open the storage for use.
        /// </summary>
        public void Open()
        {
            if (db == null)
            {
                var options = new Options { CreateIfMissing = true };
                db = new DB(options, path);
            }
        }

        public void Dispose()
        {
            if (db != null)
            {
                db.Dispose();
            }
        }

        /// <summary>
        /// Get an enumerator to iterate the storage.
        /// </summary>
        /// <returns>Enumerator that allows iterating the storage.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return db.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator to iterate the storage.
        /// </summary>
        /// <returns>Enumerator that allows iterating the storage.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return db.GetEnumerator();
        }

        /// <summary>
        /// Get an enumerator starting from the provided key to iterate the storage.
        /// </summary>
        /// <param name="key">The key to start the enumerator at.</param>
        /// <returns>The enumerator that allows iterating the storage.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator(string key)
        {
            return db.GetEnumerator(key);
        }

        /// <summary>
        /// Get the value for the given key from the storage.
        /// </summary>
        /// <param name="key">Key to look up the value of.</param>
        /// <returns>Value stored.</returns>
        public string Get(string key)
        {
            return db.Get(key);
        }

        /// <summary>
        /// Put the value for the given key in the storage.
        /// </summary>
        /// <param name="key">Key to store value for.</param>
        /// <param name="value">Value to store.</param>
        public void Put(string key, string value)
        {
            db.Put(key, value);
        }

        /// <summary>
        /// Delete the key that matches the provided key.
        /// </summary>
        /// <param name="key">Key to delete.</param>
        public void Delete(string key)
        {
            db.Delete(key);
        }

        /// <summary>
        /// Put the batch of keys and values in the storage.
        /// </summary>
        /// <param name="batch">Batch of keys and values.</param>
        public void PutBatch(Dictionary<string, string> batch)
        {
            var writeBatch = new WriteBatch();
            foreach (var column in batch)
            {
                writeBatch.Put(column.Key, column.Value);
            }
            writeBatch.Commit(db);
        }

        /// <summary>
        /// Delete the batch of keys.
        /// </summary>
        /// <param name="batch">Batch of keys to delete.</param>
        public void DeleteBatch(IEnumerable<string> batch)
        {
            var writeBatch = new WriteBatch();
            foreach (var key in batch)
            {
                writeBatch.Delete(key);
            }
            writeBatch.Commit(db);
        }
    }
}
