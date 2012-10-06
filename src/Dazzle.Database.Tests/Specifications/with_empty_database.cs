using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dazzle.Storage;

namespace Dazzle.Database.Tests.Specifications
{
    public abstract class with_empty_database : SpecificationContext
    {
        protected IStorage storage;
        protected DazzleDatabase db;

        public override void Given()
        {
            this.storage = new InMemoryStorage();
            this.db = new DazzleDatabase(this.storage);
        }
    }
}
