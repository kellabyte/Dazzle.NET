using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dazzle
{
    public interface IDazzleDatabase
    {
        void Open(string address);
        void Close();
        string Get(string key);
        void Set(string key, string value);
        void Delete(string key);
    }
}
