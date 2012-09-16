Dazzle
======
Dazzle is a database I've created to learn the internals of database architecture and design. Dazzle is a 
column-oriented database. Like Cassandra, each row can have a large number of columns (Cassandra supports billions) 
and each row can have different columns.

## Design
Like most databases, Dazzle is designed on top of a transactional storage engine. The transactional storage is 
currently LevelDB from Google which is a key/value store using a LSM (logged structured merge) with 
SSTables (Google BigTable). This is a fairly common design in databases to re-use existing transactional storage engines. 
Databases like but not limited to Riak (InnoDB, LevelDB), and RavenDB (Microsoft ESENT) also use 3rd party 
transactional storage.

Dazzle is not a relational database. Although it has a SQL-like query syntax, there is no concept of table joins.

LevelDB features:
- Keys and values are arbitrary byte arrays.
- Data is stored sorted by key.
- The basic operations are Put(key,value), Get(key), Delete(key).
- Multiple changes can be made in one atomic batch.
- Users can create a transient snapshot to get a consistent view of data (isolation).
- Forward and backward iteration is supported over the data.
- Data is automatically compressed using the Snappy compression library.

The column-oriented data model is created on top of LevelDB's key/value store similar to how SQLite4 is built 
on top of a key/value store. Dazzle currently uses a SQL-like language. The only queries supported so far are:

- select firstname, lastname, email from users where userid = 'joey'
- select * from users where userid = 'joey'

## How to use Dazzle
```csharp
using System;
using Dazzle;

namespace Dazzle.Benchmark.Embedded
{
    class Program
    {
        static void Main(string[] args)
        {
            // The database is pre-loaded with 1 million rows each with 50 columns of gibberish data.
            // This means 50+ million keys have been inserted into the key/value storage backend.
            var (var db = new DazzleDatabase("c:\\tmp")
            {
                var result = db.ExecuteQuery("select * from users where userid = 'joey'");
            }
        }
    }
}

/*
Rows: 1,000,000.00 Columns per row: 50 Total keys in DB: 50,000,000.00 DB Size: 2.36GB
Query: select * from users where userid = 'joey'

Returned Rows: 1 Columns per row: 50
Query executed in 00:00:00.0035229
*/
```

## How Dazzle works under the hood
Since Dazzle currently uses LevelDB which is a key/value store and is using SSTables (sorted string tables) 
I needed to model rows, columns and indexes in a way that would optimize sequential reads when scanning.

If we had a table called Users with columns of UserId (row key) FirstName, LastName and Email and a user with the 
UserId of 12345 the LevelDB store would look like:

```
Key                               | Value
-------------------------------------------------------------
/users                              table
/users/1234                         row
/users/1234:firstname               Adam
/users/1234:lastname                Smith
/users/1234:email                   asmith@gmail.com
/users/5678                         row
/users/5678:firstname               Bob
/users/5678:lastname                Johnson
/users/5678:email                   bjohnson@gmail.com
/users/index/firstname              index
/users/index/firstname:bob          /users/5678
```
Given the query

```
select * from users where firstname = "Bob" 
```

Dazzle's query execution plan does the following:

1. Get the value with the key "/users/index/firstname:Bob" which returns "/users/5678"
2. Seek to the key "/users/1234"
3. Create an enumerator
4. Use the enumerator to scan sequentially to retrieve the values of all the columns in the row.
5. When the enumerator scans and finds the next row the query ends and the results are returned to the client.

## Notes
- Update statements coming soon. Right now the benchmark tests pre-load data manually through LevelDB.
- Currently tested on Windows but should be trivial to port to other platforms.
- Everything is a string. Types will be introduced soon.

None of this is set in stone. My learning may lead me in different directions during this project.
 