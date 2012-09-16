using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Dazzle.Query;
using Dazzle.Query.Operations;
using Irony.Parsing;
using LevelDB;
using SimpleInjector;
using SimpleInjector.Extensions;

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
        private string path;
        private DB db;

        public DazzleDatabase(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("path");
            }
            this.path = path;
            Initialize();
        }

        ~DazzleDatabase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                this.Close();
                disposed = true;
            }
        }

        /// <summary>
        /// Initializes the composition of the <see cref="DazzleDatabase"/>
        /// </summary>
        private void Initialize()
        {
            // Composition root.
            container = new Container();
            container.Register<Grammar, DqlGrammar>();
            container.Register<LanguageData>();
            container.Register<IQueryProcessor, LevelDbQueryProcessor>();
            container.Register<DqlQueryReader>();
            container.Register<DqlQueryPlanBuilder>();
            container.RegisterAll(typeof(IOperationBuilder), new Type[] { typeof(SelectOperationBuilder) });
        }

        /// <summary>
        /// Open the database connection.
        /// </summary>
        private void Open()
        {
            var options = new Options { CreateIfMissing = true };
            db = new DB(options, path);
            opened = true;
            container.RegisterSingle<DB>(db);
        }

        /// <summary>
        /// Close the database connection.
        /// </summary>
        private void Close()
        {
            if (db != null)
            {
                db.Dispose();
                opened = false;
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

            if (!opened)
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
