namespace Dazzle.Query
{
    /// <summary>
    /// Processor that executes Dql.
    /// </summary>
    public interface IQueryProcessor
    {
        /// <summary>
        /// Executes the specified Dql.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The results from executing the query.</returns>
        QueryResult Execute(string query);
    }
}