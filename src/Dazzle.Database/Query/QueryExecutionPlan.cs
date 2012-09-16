using System;
using System.Collections.Generic;
using Dazzle.Query.Operations;
using LevelDB;

namespace Dazzle.Query
{
    /// <summary>
    /// The query execution plan
    /// </summary>
    public class QueryExecutionPlan
    {
        private DB db;

        public QueryExecutionPlan(DB db)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }

            this.db = db;
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
                // TODO: Row shouldn't be created here.
                var results = operation.Execute(db);
                var row = new Row();
                foreach (var r in results)
                {
                    row.Columns.Add(r.Key, r.Value);
                }
                result.Rows.Add(row);
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