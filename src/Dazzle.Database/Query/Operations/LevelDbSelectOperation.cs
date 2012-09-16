using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using LevelDB;

namespace Dazzle.Query.Operations
{
    /// <summary>
    /// Represents a select operation against the database.
    /// </summary>
    public class LevelDbSelectOperation : IQueryOperation
    {
        // /table/index/column:id
        private const string SEEK_ROW_BY_INDEX = "/{0}/index/{1}:{2}";

        private bool selectAll;

        public LevelDbSelectOperation()
        {
            this.ColumnNames = new List<String>();
            this.WhereClauses = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets and sets the name of the table the <see cref="IQueryOperation"/> will execute against.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets the names of the columns in the table involved in the <see cref="IQueryOperation"/>.
        /// </summary>
        public List<string> ColumnNames { get; private set; }

        /// <summary>
        /// Gets the column and criteria where clauses involved in the <see cref="IQueryOperation"/>.
        /// </summary>
        public Dictionary<string, string> WhereClauses { get; private set; }

        /// <summary>
        /// Execute the <see cref="IQueryOperation"/> against the database.
        /// </summary>
        /// <param name="db">Database to execute against.</param>
        /// <returns>Query results.</returns>
        [HandleProcessCorruptedStateExceptions]
        public Dictionary<string, string> Execute(DB db)
        {
            // TODO: This needs to handle getting more than one result row.
            var results = new Dictionary<string, string>();
            var columns = new List<string>(this.ColumnNames);

            if (columns.Contains("*"))
            {
                this.selectAll = true;
            }

            string key = string.Format(
                SEEK_ROW_BY_INDEX, 
                this.TableName,
                this.WhereClauses.Keys.ToArray()[0], 
                this.WhereClauses.Values.ToArray()[0]);

            var val = db.Get(key);
            if (val == null)
            {
                return results;
            }
            var enumerator = db.GetEnumerator(val);

            // TODO: Remove try. For some reason we are getting a pinvoke memory access violation.
            try
            {
                while (enumerator.MoveNext())
                {
                    var paths = enumerator.Current.Key.Split('/', ':');
                    if (enumerator.Current.Value != "row" && enumerator.Current.Value != "index" && paths[2] != "index")
                    {
                        var column = paths[paths.Length - 1];
                        if (selectAll || columns.Contains(column))
                        {
                            results.Add(column, enumerator.Current.Value);
                            columns.Remove(column);

                            if (columns.Count == 0)
                            {
                                // Exit since we have read all the columns from the query.
                                break;
                            }
                        }
                    }

                    if (enumerator.Current.Key != val && enumerator.Current.Value == "row")
                    {
                        // We've encountered a new row, exit.
                        break;
                    }
                }
            }
            catch (AccessViolationException e)
            {
                // TODO: I should do something here.
            }
            
            return results;
        }
    }
}