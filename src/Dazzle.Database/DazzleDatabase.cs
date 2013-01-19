using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Irony.Parsing;
using SimpleInjector;
using SimpleInjector.Extensions;
using Dazzle.Operations;
using Dazzle.Query;
using Dazzle.Storage;

namespace Dazzle
{
    /// <summary>
    /// The Dazzle database.
    /// </summary>
    public class DazzleDatabase : IDisposable
    {
        private bool disposed;
        private bool opened = false;
        private Container container;
        private IStorage storage;

        public DazzleDatabase(string path)
            : this(new LevelDBStorage(path))
        {
        }

        public DazzleDatabase(IStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }
            this.storage = storage;
            this.Initialize();
            this.Open();
        }

        ~DazzleDatabase()
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
                this.Close();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Initializes the composition of the <see cref="DazzleDatabase"/>
        /// </summary>
        private void Initialize()
        {
            // Composition root.
            this.container = new Container();
            this.container.Register<Grammar, DqlGrammar>();
            this.container.Register<LanguageData>();
            this.container.Register<IQueryProcessor, QueryProcessor>();
            this.container.Register<DqlQueryReader>();
            this.container.Register<DqlQueryPlanBuilder>();
            this.container.RegisterAll(typeof(IOperationBuilder), new Type[]
                {
                    typeof(SelectOperationBuilder),
                    typeof(UpdateOperationBuilder),
                    typeof(DeleteOperationBuilder),
                });
        }

        /// <summary>
        /// Open the database connection.
        /// </summary>
        private void Open()
        {
            this.container.RegisterSingle<IStorage>(storage);
            this.storage.Open();
            this.opened = true;
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        private void Close()
        {
            if (storage != null)
            {
                this.storage.Dispose();
                this.opened = false;
            }
        }

        /// <summary>
        /// Execute Dql query and return results.
        /// </summary>
        /// <param name="query">Dql query.</param>
        /// <returns>Rows and columns as a result of the Dql query.</returns>
        public QueryResult ExecuteQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            var watch = new Stopwatch();
            watch.Start();

            if (!this.opened)
            {
                this.Open();
            }
            
            var queryProcessor = container.GetInstance<IQueryProcessor>();
            var result = queryProcessor.Execute(query);
            watch.Stop();
            result.ExecutionTime = watch.Elapsed;

            return result;
        }
    }
}
