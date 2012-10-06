using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dazzle.Storage
{
    public interface IStorage : IEnumerable<KeyValuePair<string, string>>, IDisposable
    {
        void Open();
        string Get(string key);
        void Put(string key, string value);
        void PutBatch(Dictionary<string, string> batch);
        IEnumerator<KeyValuePair<string, string>> GetEnumerator();
        IEnumerator<KeyValuePair<string, string>> GetEnumerator(string key);
    }
}
