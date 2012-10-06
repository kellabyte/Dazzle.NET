using System.Collections.Generic;
using Dazzle.Storage;

namespace Dazzle.Operations
{
    /// <summary>
    /// Represents an operation against the database.
    /// </summary>
    public interface IQueryOperation
    {
        /// <summary>
        /// Execute the <see cref="IQueryOperation"/> against the database.
        /// </summary>
        /// <param name="db">Database to execute against.</param>
        /// <returns>Query results.</returns>
        IEnumerable<Row> Execute(IStorage storage);
    }
}