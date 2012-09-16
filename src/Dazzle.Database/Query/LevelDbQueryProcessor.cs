using System;
using Irony.Parsing;
using LevelDB;

namespace Dazzle.Query
{
    /// <summary>
    /// Query processor that executes Dql on LevelDB.
    /// </summary>
    public class LevelDbQueryProcessor : IQueryProcessor
    {
        private readonly DB db;
        private readonly DqlQueryReader reader;
        private readonly DqlQueryPlanBuilder optimizer;

        public LevelDbQueryProcessor(DB db, DqlQueryReader reader, DqlQueryPlanBuilder optimizer)
        {
            if (db == null)
            {
                throw new ArgumentNullException("db");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (optimizer == null)
            {
                throw new ArgumentNullException("optimizer");
            }

            this.db = db;
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
            var plan = optimizer.BuildQueryExecutionPlan(syntaxTree.Root, new QueryExecutionPlan(db));
            var result = plan.Execute();
            return result;
        }
    }
}