using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Dazzle.Storage;

namespace Dazzle.Operations
{
    /// <summary>
    /// Represents a select operation against the database.
    /// </summary>
    public class SelectOperation : IQueryOperation
    {
        private const string SEEK_ROW_BY_INDEX = "/{0}/$index/{1}/{2}";

        private bool selectAll;

        public SelectOperation()
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
        /// <param name="storage">Database to execute against.</param>
        /// <returns>Query results.</returns>
        [HandleProcessCorruptedStateExceptions]
        public IEnumerable<Row> Execute(IStorage storage)
        {
            // 1. Reduce selection by evaluating where clauses.
            var rows = new List<Row>();
            var sets = new List<HashSet<string>>();

            for (int i=0; i<this.WhereClauses.Count; i++)
            {
                var whereClause = this.WhereClauses.ElementAt(i);
                var set = new HashSet<string>();
                sets.Add(set);

                // First try by using an index lookup and scan.
                string key = string.Format(
                    SEEK_ROW_BY_INDEX,
                    this.TableName,
                    whereClause.Key,
                    whereClause.Value);

                var enumerator = storage.GetEnumerator(key);
                if (enumerator != null)
                {
                    bool cancelIndexScanning = false;
                    try
                    {
                        while (!cancelIndexScanning && enumerator.MoveNext())
                        {
                            var keyComponents = enumerator.Current.Key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                            var value = enumerator.Current.Value;
                            var keyType = GetKeyType(keyComponents, value);

                            switch (keyType)
                            {
                                case KeyType.IndexedTerm:
                                    if (keyComponents[2] != whereClause.Key)
                                    {
                                        cancelIndexScanning = true;
                                    }
                                    break;
                                case KeyType.IndexedValue:
                                    if (keyComponents[3] == whereClause.Value)
                                    {
                                        set.Add(value);
                                    }
                                    break;
                            }
                        }
                    }
                    catch (AccessViolationException e)
                    {
                        // TODO: Should do something here.
                    }
                    finally
                    {
                        enumerator.Dispose();
                    }
                }
            }

            // Intersect the results from the where clauses to find the final result set.
            var intersection = IntersectAll<string>(sets);
            intersection.Sort();

            // 2. Select the rows
            if (this.ColumnNames.Contains("*"))
            {
                this.selectAll = true;
            }

            var added = new List<string>();

            foreach (var key in intersection)
            {
                IEnumerator<KeyValuePair<string, string>> enumerator = null;
                try
                {
                    enumerator = storage.GetEnumerator(key);
                    enumerator.MoveNext();

                    string[] rowKeyComponents = key.Split('/');
                    var row = new Row();
                    row.RowKey = rowKeyComponents[rowKeyComponents.Length - 1];
                    row.Columns.Add("key", row.RowKey);
                    rows.Add(row);

                    // Select the columns.
                    while (enumerator.MoveNext())
                    {
                        var keyComponents = enumerator.Current.Key.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        var keyType = GetKeyType(keyComponents, enumerator.Current.Value);

                        if (keyType == KeyType.Row)
                        {
                            break;
                        }
                        else if (keyType == KeyType.ColumnValue && (selectAll || this.ColumnNames.Contains(keyComponents[3])))
                        {
                            // If all the columns are selected or this column is specified
                            // in the select clause add it to the results.
                            row.Columns.Add(keyComponents[3], enumerator.Current.Value);
                        }
                    }
                }
                catch (AccessViolationException e)
                {
                    // TODO: Should do something here.
                }
                finally
                {
                    if (enumerator != null)
                    {
                        enumerator.Dispose();
                    }
                }
            }
            return rows;
        }

        /// <summary>
        /// Intersect multiple <see cref="HashSet"/> together.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="HashSet"/>.</typeparam>
        /// <param name="lists">Collection of <see cref="HashSet"/></param>
        /// <returns>Intersected <see cref="HashSet"/></returns>
        public static List<T> IntersectAll<T>(IEnumerable<IEnumerable<T>> lists)
        {
            HashSet<T> hashSet = new HashSet<T>(lists.First());
            foreach (var list in lists.Skip(1))
            {
                hashSet.IntersectWith(list);
            }
            return hashSet.ToList();
        }

        /// <summary>
        /// Get the <see cref="KeyType"/> stored for the given components of a key and key value.
        /// </summary>
        /// <param name="keyComponents">Components of a stored key.</param>
        /// <param name="value">Value of the stored key.</param>
        /// <returns>The <see cref="KeyType"/></returns>
        private static KeyType GetKeyType(string[] keyComponents, string value)
        {
            switch (value)
            {
                case "table":
                    return KeyType.Table;
                case "row":
                    return KeyType.Row;
                case "column":
                    return KeyType.Column;
                case "indexed-column":
                    return KeyType.IndexedColumn;
                case "indexed-term":
                    return KeyType.IndexedTerm;
            }

            if (keyComponents[1] == "$index")
            {
                return KeyType.IndexedValue;
            }
            else if (keyComponents[2] == "$column")
            {
                return KeyType.ColumnValue;
            }
            return KeyType.Unknown;
        }
    }
}