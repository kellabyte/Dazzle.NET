using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dazzle.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dazzle.Database.Tests
{
    public abstract class SpecificationContext : IDisposable
    {
        private bool disposed;
        protected IStorage storage;
        protected DazzleDatabase db;

        [TestInitialize]
        public void Init()
        {
            this.storage = new InMemoryStorage();
            this.db = new DazzleDatabase(this.storage);

            this.Given();
            this.When();
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.FinishingUp();
        }

        public virtual void Given() { }
        public virtual void When() { }

        public virtual void FinishingUp() { }

        ~SpecificationContext()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                this.disposed = true;
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
            }
        }
    }
}
