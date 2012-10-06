using System;
using Dazzle.Storage;
using Irony.Parsing;

namespace Dazzle.Query
{
    /// <summary>
    /// Query processor that executes Dql on against the storage.
    /// </summary>
    public class QueryProcessor : IQueryProcessor
    {
        private readonly IStorage storage;
        private readonly DqlQueryReader reader;
        private readonly DqlQueryPlanBuilder optimizer;

        public QueryProcessor(IStorage storage, DqlQueryReader reader, DqlQueryPlanBuilder optimizer)
        {
            if (storage == null)
            {
                throw new ArgumentNullException("storage");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (optimizer == null)
            {
                throw new ArgumentNullException("optimizer");
            }

            this.storage = storage;
            this.reader = reader;
            this.optimizer = optimizer;
        }

        /// <summary>
        /// Executes the specified Dql on LevelDB.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The results from executing the query.</returns>
        public QueryResult Execute(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException("query");
            }

            ParseTree syntaxTree;
            var parseTime = reader.Read(query, out syntaxTree);
            var plan = optimizer.BuildQueryExecutionPlan(syntaxTree.Root, new QueryExecutionPlan(storage));
            var result = plan.Execute();
            return result;
        }
    }
}