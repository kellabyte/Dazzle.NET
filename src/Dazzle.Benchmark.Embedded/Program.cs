using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Text;
using Dazzle;
using Dazzle.Extensions;
using Dazzle.Server;
using Dazzle.Storage;
using LevelDB;

namespace Dazzle.Benchmark.Embedded
{
    internal class Program
    {
        // Set this to true if you want to generate data the first time.
        private static bool preload = true;

        private static string path = "c:\\tmp";
        private static int rows = 2;
        private static int columns = 2;

        private static void Main(string[] args)
        {
            if (preload && Directory.Exists(path))
                Directory.Delete(path, true);

            using (var storage = new InMemoryStorage())
            using (var db = new DazzleDatabase(storage))
            {
                if (preload)
                {
                    PreloadData(storage);
                    ShowData(storage);
                }

                var result1 = ExecuteQuery(db, "select * from users where column0 = 'bob1' and column1 = 'bob1'");
                var result2 = ExecuteQuery(db, "select * from users where column0 = 'bob1' and column1 = 'bob1'");
                //var result2 = ExecuteQuery(db, "update users set column0 = 'bob8888', column1 = 'bob8888' where key = 1");
                //var result3 = ExecuteQuery(db, "select * from users where column0 = 'bob9999' and column1 = 'bob9999'");
                //var result2 = ExecuteQuery(db, "select column0, column1, column49 from users where column0 = 'bob1' and column1 = 'bob1'");

                //var result2 = ExecuteQuery(db, "select * from users where column0 = 'bob1' and column1 = 'bob1'");
                //var result2 = ExecuteQuery(db, "select * from users where column0 = 'bob1' and column49 = 'bob1'");
                //var result3 = ExecuteQuery(db, "select column15 from users where column0 = 'bob1'");
                //var result1 = ExecuteQuery(db, "select * from users where column0 = 'bob1'");
            }

            //var client = new System.Net.Sockets.TcpClient();
            //client.Connect("127.0.0.1", 5223);

            //string query = "select * from users where column49 = 'bob9000'";
            //var data = new byte[query.Length + 3];
            //data[0] = (byte)MessageType.Query;
            //var length = BitConverter.GetBytes(query.Length);
            //length.CopyTo(data, 1);
            //var queryBytes = UTF8Encoding.UTF8.GetBytes(query);
            //queryBytes.CopyTo(data, 3);
            //var buffer = new byte[1024];

            //int count = 10000;
            //var watch = new Stopwatch();

            //watch.Start();
            //for (int i = 0; i < count; i++)
            //{
            //    client.Client.Send(data);
            //    int read = client.Client.Receive(buffer);
            //}
            //watch.Stop();

            //Console.WriteLine(count / watch.Elapsed.TotalSeconds);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PreloadData(IStorage storage)
        {
            var watch = new Stopwatch();
            watch.Start();

            string table = "users";
            var batch = new Dictionary<string, string>();
            //batch.TryAdd(string.Format("/{0}", table), "table");
            batch[string.Format("/{0}", table)] = "table";

            for (int i = 0; i < rows; i++)
            {
                var rowKey = i.ToString();
                //batch.TryAdd(string.Format("/{0}/{1}", table, rowKey), "row");
                batch[string.Format("/{0}/{1}", table, rowKey)] = "row";

                for (int x = 0; x < columns; x++)
                {
                    string columnKey = "column" + x;
                    string columnValue = "bob1";

                    // Add column indexes. Redundant calls are idempotent.
                    if (i == 0)
                    {
                        //batch.TryAdd(string.Format("/{0}/$index/{1}", table, columnKey), "indexed-column");
                        batch[string.Format("/{0}/$index/{1}", table, columnKey)] = "indexed-column";
                    }

                    // /users/0:column0, bob0
                    //batch.TryAdd(string.Format("/{0}/{1}/$column/{2}", table, rowKey, columnKey), columnValue);
                    batch[string.Format("/{0}/{1}/$column/{2}", table, rowKey, columnKey)] = columnValue;

                    // /users/$index/column0/bob1, indexed-term
                    //batch.TryAdd(string.Format("/{0}/$index/{1}/{2}", table, columnKey, columnValue), "indexed-term");
                    batch[string.Format("/{0}/$index/{1}/{2}", table, columnKey, columnValue)] = "indexed-term";

                    // Indexed value
                    // /users/$index/column0/bob1/1, /users/1
                    //batch.TryAdd(string.Format("/{0}/$index/{1}/{2}/{3}", table, columnKey, columnValue, rowKey),
                    //          string.Format("/{0}/{1}", table, rowKey));
                    batch[string.Format("/{0}/$index/{1}/{2}/{3}", table, columnKey, columnValue, rowKey)] =
                        string.Format("/{0}/{1}", table, rowKey);

                }
            }
            storage.PutBatch(batch);

            /*
            string table = "users";
            batch.Put(string.Format("/{0}", table), "table");

            for (int i = 0; i < rows; i++)
            {
                var rowKey = i.ToString();
                var cols = new Dictionary<string, string>();
                batch.Put(string.Format("/{0}/{1}", table, rowKey), "row");

                for (int x = 0; x < columns; x++)
                {
                    cols.Add("column" + x, "bob1");
                }
                AddRow(batch, "users", rowKey, cols);
            }
            */
            //batch.Commit(leveldb);
            //leveldb.Dispose();
            watch.Stop();

            Console.WriteLine(
                "{0} Rows: " + string.Format("{0:N}", rows) +
                " Columns per row: " + columns +
                " Keys in DB: " + string.Format("{0:N}", rows * columns), watch.Elapsed);
        }

        private static void AddRow(WriteBatch batch, string table, string rowKey, Dictionary<string, string> columns)
        {
            foreach (var column in columns)
            {
                // Add column indexes. Redundant calls are idempotent.
                batch.Put(string.Format("/{0}/$index/{1}", table, column.Key), "indexed-column");

                // /users/0:column0, bob0
                batch.Put(string.Format("/{0}/{1}/$column/{2}", table, rowKey, column.Key), column.Value);

                // /users/$index/column0/bob1, indexed-term
                batch.Put(string.Format("/{0}/$index/{1}/{2}", table, column.Key, column.Value), "indexed-term");

                // Indexed value
                // /users/$index/column0/bob1/1, /users/1
                batch.Put(string.Format("/{0}/$index/{1}/{2}/{3}", table, column.Key, column.Value, rowKey),
                    string.Format("/{0}/{1}", table, rowKey));
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private static void ShowData(IStorage storage)
        {
            //var options = new Options();
            //options.CreateIfMissing = true;
            //var leveldb = new DB(options, path);

            try
            {
                Console.WriteLine("-------------------------------------------------------------------");
                Console.WriteLine("Key                                  | Value");
                Console.WriteLine("-------------------------------------------------------------------");

                var enumerator = storage.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    //Console.WriteLine(enumerator.Current.Key + "\t" + enumerator.Current.Value);
                    Console.WriteLine(String.Format("{0,-36} | {1,-36}", enumerator.Current.Key, enumerator.Current.Value));
                }
            }
            catch (AccessViolationException e)
            {
                // TODO: Should do something here.
            }
            //leveldb.Dispose();
        }

        private static QueryResult ExecuteQuery(DazzleDatabase db, string query)
        {
            Console.WriteLine();
            Console.WriteLine("Query: " + query);
                
            var result = db.ExecuteQuery(query);
            if (result.Rows.Count > 0)
            {
                Console.WriteLine("Returned Rows: " + result.Rows.Count + " Columns per row: " +
                                  result.Rows[0].Columns.Count);
            }
            Console.WriteLine("Query executed in " + result.ExecutionTime);
            return result;
        }
    }
}
