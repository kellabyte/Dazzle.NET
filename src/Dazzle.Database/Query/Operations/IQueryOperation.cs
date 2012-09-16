using System.Collections.Generic;
using LevelDB;

namespace Dazzle.Query.Operations
{
    /// <summary>
    /// Represents an operation against the database.
    /// </summary>
    public interface IQueryOperation
    {
        /// <summary>
        /// Gets and sets the name of the table the <see cref="IQueryOperation"/> will execute against.
        /// </summary>
        string TableName { get; set; }

        /// <summary>
        /// Gets the names of the columns in the table involved in the <see cref="IQueryOperation"/>.
        /// </summary>
        List<string> ColumnNames { get; }

        /// <summary>
        /// Gets the column and criteria where clauses involved in the <see cref="IQueryOperation"/>.
        /// </summary>
        Dictionary<string, string> WhereClauses { get; }

        /// <summary>
        /// Execute the <see cref="IQueryOperation"/> against the database.
        /// </summary>
        /// <param name="db">Database to execute against.</param>
        /// <returns>Query results.</returns>
        Dictionary<string, string> Execute(DB db);
    }
}