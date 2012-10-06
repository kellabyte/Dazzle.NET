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

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return db.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return db.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator(string key)
        {
            return db.GetEnumerator(key);
        }

        public string Get(string key)
        {
            return db.Get(key);
        }

        public void Put(string key, string value)
        {
            db.Put(key, value);
        }

        public void PutBatch(Dictionary<string, string> batch)
        {
            var writeBatch = new WriteBatch();
            foreach (var column in batch)
            {
                writeBatch.Put(column.Key, column.Value);
            }
            writeBatch.Commit(db);
        }
    }
}
