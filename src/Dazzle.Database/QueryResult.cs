using System;
using System.Collections.Generic;

namespace Dazzle
{
    /// <summary>
    /// Row of results.
    /// </summary>
    public class Row
    {
        public Row()
        {
            this.Columns = new Dictionary<string, string>();
        }

        /// <summary>
        /// Row key of this row.
        /// </summary>
        public string RowKey { get; set; }

        /// <summary>
        /// Columns for this row returned from the query.
        /// </summary>
        public Dictionary<string, string> Columns { get; internal set; }
    }

    /// <summary>
    /// Results from a Dazzle query.
    /// </summary>
    public class QueryResult
    {
        public QueryResult()
        {
            this.Rows = new List<Row>();
            this.ExecutionTime = new TimeSpan();
        }

        /// <summary>
        /// Rows in the results.
        /// </summary>
        public List<Row> Rows { get; internal set; }

        /// <summary>
        /// Time taken to execute query.
        /// </summary>
        public TimeSpan ExecutionTime { get; internal set; }
    }
}