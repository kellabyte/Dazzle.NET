using System;
using System.IO;
using Dazzle;
using LevelDB;

namespace Dazzle.Benchmark.Embedded
{
    internal class Program
    {
        // Set this to true if you want to generate data the first time.
        private static bool preload = true;

        private static string path = "c:\\tmp";
        private static int rows = 10000;
        private static int columns = 50;

        private static void Main(string[] args)
        {            
            if (preload)
                PreloadData();

            using (var db = new DazzleDatabase(path))
            {
                var result1 = ExecuteQuery(db, "select * from users where column49 = 'bob9000'");
                var result2 = ExecuteQuery(db, "select * from users where column49 = 'bob9000'");
                var result3 = ExecuteQuery(db, "select column0, column1, column2, column3, column4 from users where column49 = 'bob9000'");
            }
            Console.ReadKey();
        }

        private static void PreloadData()
        {
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            var options = new Options();
            options.CreateIfMissing = true;
            var leveldb = new DB(options, path);

            var id = Guid.NewGuid().ToString();
            string rowkey = "bob";

            leveldb.Put("/users", "table");
            leveldb.Put("/users/index/column0", "index");
            leveldb.Put("/users/index/column49", "index");

            for (int i = 0; i < rows; i++)
            {
                leveldb.Put(string.Format("/users/{0}", i), "row");
                for (int x = 0; x < columns; x++)
                {
                    leveldb.Put(string.Format("/users/{0}:column{1}", i, x), "bob" + i);
                    leveldb.Put(string.Format("/users/index/column{0}:{1}", x, "bob" + i),
                                string.Format("/users/{0}", i));
                }
            }
            leveldb.Dispose();

            Console.WriteLine(
                "Rows created: " + string.Format("{0:N}", rows) +
                " Columns per row: " + columns +
                " Total keys in DB: " + string.Format("{0:N}", rows * columns));
        }

        private static QueryResult ExecuteQuery(DazzleDatabase db, string query)
        {
            Console.WriteLine();
            Console.WriteLine("Query: " + query);
                
            var result = db.ExecuteQuery(query);
                
            Console.WriteLine("Returned Rows: " + result.Rows.Count + " Columns per row: " +
                              result.Rows[0].Columns.Count);
            Console.WriteLine("Query executed in " + result.ExecutionTime);
            return result;
        }
    }
}
