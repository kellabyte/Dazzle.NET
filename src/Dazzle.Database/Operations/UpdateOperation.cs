using System.Collections.Generic;
using System.Linq;
using Dazzle.Storage;
using LevelDB;

namespace Dazzle.Operations
{
    /// <summary>
    /// Represents an update operation against the database.
    /// </summary>
    public class UpdateOperation : IQueryOperation
    {
        public UpdateOperation()
        {
            this.Assignments = new Dictionary<string, string>();
            this.WhereClauses = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets and sets the assignments to be updated to a row.
        /// </summary>
        public Dictionary<string, string> Assignments { get; private set; }

        /// <summary>
        /// Gets and sets the name of the table the <see cref="IQueryOperation"/> will execute against.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets the column and criteria where clauses involved in the <see cref="IQueryOperation"/>.
        /// </summary>
        public Dictionary<string, string> WhereClauses { get; private set; }

        /// <summary>
        /// Execute the <see cref="IQueryOperation"/> against the database.
        /// </summary>
        /// <param name="storage">Database to execute against.</param>
        /// <returns>Query results.</returns>
        public IEnumerable<Row> Execute(IStorage storage)
        {
            var rows = new List<Row>();
            if (this.WhereClauses.Count == 1 && this.WhereClauses.Keys.ElementAt(0) == "key")
            {
                // Optimize for single row update by row key.
                UpdateRow(storage, this.TableName, this.WhereClauses.Values.ElementAt(0), this.Assignments);
            }            
            return rows;
        }

        /// <summary>
        /// Stores a row and all its columns using the storage.
        /// </summary>
        /// <param name="storage">The storage to store the row and columns in.</param>
        /// <param name="table">The table the row belongs to.</param>
        /// <param name="rowKey">The row key of the row.</param>
        /// <param name="columns">The collection of columns to store for the row.</param>
        private static void UpdateRow(IStorage storage, string table, string rowKey, Dictionary<string, string> columns)
        {
            var keys = new Dictionary<string, string>();

            // Add row key.
            keys.Add(string.Format("/{0}/{1}", table, rowKey), "row");

            foreach (var column in columns)
            {
                // Add column indexes. Redundant calls are idempotent.
                keys.Add(string.Format("/{0}/$index/{1}", table, column.Key), "indexed-column");

                // /users/0:column0, bob0
                keys.Add(string.Format("/{0}/{1}/$column/{2}", table, rowKey, column.Key), column.Value);

                // /users/$index/column0/bob1, indexed-term
                keys.Add(string.Format("/{0}/$index/{1}/{2}", table, column.Key, column.Value), "indexed-term");

                // Indexed value
                // /users/$index/column0/bob1/1, /users/1
                keys.Add(string.Format("/{0}/$index/{1}/{2}/{3}", table, column.Key, column.Value, rowKey),
                    string.Format("/{0}/{1}", table, rowKey));
            }
            storage.PutBatch(keys);
        }
    }
}