using System;
using System.Collections.Generic;
using Dazzle.Operations;
using Dazzle.Storage;

namespace Dazzle.Query
{
    /// <summary>
    /// The query execution plan
    /// </summary>
    public class QueryExecutionPlan
    {
        private IStorage storage;

        public QueryExecutionPlan(IStorage storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }

            this.storage = storage;
            this.Operations = new List<IQueryOperation>();
        }

        /// <summary>
        /// Executes the <see cref="QueryExecutionPlan"/> on the database.
        /// </summary>
        /// <returns>Results from the query execution.</returns>
        public QueryResult Execute()
        {
            var result = new QueryResult();
            foreach (var operation in this.Operations)
            {
                var rows = operation.Execute(this.storage);
                result.Rows.InsertRange(result.Rows.Count, rows);
            }
            return result;
        }

        /// <summary>
        /// Operations to be executed in the <see cref="QueryExecutionPlan"/>.
        /// </summary>
        public List<IQueryOperation> Operations { get; private set; }

        /// <summary>
        /// Current operation under construction.
        /// </summary>
        public IQueryOperation Current
        {
            get
            {
                if (this.Operations.Count > 0)
                {
                    return this.Operations[this.Operations.Count - 1];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}