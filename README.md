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

```
-------------------------------------------------------------------
Key                                  | Value
-------------------------------------------------------------------
/users                               | table
/users/$index/column0                | indexed-column
/users/$index/column0/bob1           | indexed-term
/users/$index/column0/bob1/0         | /users/0
/users/$index/column0/bob1/1         | /users/1
/users/$index/column1                | indexed-column
/users/$index/column1/bob1           | indexed-term
/users/$index/column1/bob1/0         | /users/0
/users/$index/column1/bob1/1         | /users/1
/users/0                             | row
/users/0/$column/column0             | bob1
/users/0/$column/column1             | bob1
/users/1                             | row
/users/1/$column/column0             | bob1
/users/1/$column/column1             | bob1
```
Given the query

```
select * from users where column0 = "bob1" 
```
Dazzle's query execution plan does the following:

1. Seek to the key "/users/$index/column0/bob1"
2. Scan down to collect all the row keys for rows that are in this indexed column such as /users/0 and /users/1
3. For each row key collected seek to the row such as /users/0
4. Scan the columns for the row and collect the columns and their values that are defined in the select clause or all if * was specified.
5. When the enumerator scans and finds the next row the query repeats #3 and #4 for the next rows.

## Notes
- Currently tested on Windows but should be trivial to port to other platforms.
- Everything is a string. Types will be introduced soon.

None of this is set in stone. My learning may lead me in different directions during this project.

## TODO
There's no real hard set of TODO's because I'm not 100% sure what type of database I want to build. My learning is
guiding me but here's a list of things I know missing and other possibilities.

- Implement Create Database command.
  - Right now there's no real concept of a database.
- Implement Create Table command.
  - Right now the DB has no real concept of a table.
- Implement Delete command.
- Handling query errors.
  - Query engine is assuming success and that the table, columns specified actually exist. Need to do checks & throw errors.
- Add TCP networking so that Dazzle can be a service as well as an embedded database.
- Integrating Ring.io into Dazzle so that Dazzle becomes a distributed hash table (consistent hashing) cluster using Gossip protocol.