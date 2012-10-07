using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dazzle.Database.Tests
{
    [TestClass]
    public class when_deleting_where_firstname_equals_bob : with_database_having_2_rows
    {
        private QueryResult results;

        public override void When()
        {
            results = db.ExecuteQuery("delete from users where firstname = 'bob' and lastname='smith'");
        }

        [TestMethod]
        public void then_bob_indexed_term_should_still_exist()
        {
            Assert.AreEqual("indexed-term", storage.Get("/users/$index/firstname/bob"));
        }

        [TestMethod]
        public void then_smith_indexed_term_should_still_exist()
        {
            Assert.AreEqual("indexed-term", storage.Get("/users/$index/lastname/smith"));
        }

        [TestMethod]
        public void then_firstname_indexed_row_for_rowkey_2_should_still_exist()
        {
            Assert.AreEqual(null, storage.Get("/users/$index/firstname/bob/2"));
        }

        [TestMethod]
        public void then_lastname_indexed_row_for_rowkey_2_should_still_exist()
        {
            Assert.AreEqual(null, storage.Get("/users/$index/lastname/smith/2"));
        }

        [TestMethod]
        public void then_row_with_rowkey_2_should_be_deleted()
        {
            Assert.AreEqual(null, storage.Get("/users/2"));
        }

        [TestMethod]
        public void then_firstname_for_row_with_rowkey_2_should_be_deleted()
        {
            Assert.AreEqual(null, storage.Get("/users/2/$column/firstname"));
        }

        [TestMethod]
        public void then_lastname_for_row_with_rowkey_2_should_be_deleted()
        {
            Assert.AreEqual(null, storage.Get("/users/2/$column/lastname"));
        }
    }
}
