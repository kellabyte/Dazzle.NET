using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dazzle.Storage;

namespace Dazzle.Operations
{
    /// <summary>
    /// Represents aa delete operation against the database.
    /// </summary>
    public class DeleteOperation : IQueryOperation
    {
        public DeleteOperation()
        {
            this.WhereClauses = new Dictionary<string, string>();
        }

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
            if (this.WhereClauses.Count > 0)
            {
                var select = new SelectOperation();
                select.TableName = this.TableName;
                select.ColumnNames.Add("*");

                foreach (var clause in this.WhereClauses)
                {
                    select.WhereClauses.Add(clause.Key, clause.Value);
                }

                var selectedRows = select.Execute(storage);
                foreach (var row in selectedRows)
                {
                    DeleteRow(storage, this.TableName, row.RowKey, row.Columns);
                }
            }
            return rows;
        }

        /// <summary>
        /// Deletes a row and all its columns using the storage.
        /// </summary>
        /// <param name="storage">The storage to delete the row and columns from.</param>
        /// <param name="table">The table the row belongs to.</param>
        /// <param name="rowKey">The row key of the row.</param>
        /// <param name="columns">The collection of columns to delete for the row.</param>
        private static void DeleteRow(IStorage storage, string table, string rowKey, Dictionary<string, string> columns)
        {
            var keys = new List<string>();

            // Delete row key.
            keys.Add(string.Format("/{0}/{1}", table, rowKey));

            foreach (var column in columns)
            {
                // Indexed row
                keys.Add(string.Format("/{0}/{1}/$column/{2}", table, rowKey, column.Key));

                // Indexed value
                keys.Add(string.Format("/{0}/$index/{1}/{2}/{3}", table, column.Key, column.Value, rowKey));
            }
            storage.DeleteBatch(keys);
        }
    }
}
