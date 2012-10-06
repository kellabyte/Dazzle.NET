using System;
using Dazzle.Database.Tests.Specifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dazzle.Storage;

namespace Dazzle.Database.Tests
{
    [TestClass]
    public class when_updating_firstname_jane_and_lastname_tompson_where_key_1 : with_empty_database
    {
        public override void When()
        {
            db.ExecuteQuery("update users set firstname = 'jane', lastname = 'tompson' where key = 1");
        }

        [TestMethod]
        public void then_firstname_indexed_column_should_be_created()
        {
            Assert.AreEqual("indexed-column", storage.Get("/users/$index/firstname"));
        }

        [TestMethod]
        public void then_lastname_indexed_column_should_be_created()
        {
            Assert.AreEqual("indexed-column", storage.Get("/users/$index/lastname"));
        }

        [TestMethod]
        public void then_firstname_jane_indexed_term_should_be_created()
        {
            Assert.AreEqual("indexed-term", storage.Get("/users/$index/firstname/jane"));
        }

        [TestMethod]
        public void then_lastname_tompson_indexed_term_should_be_created()
        {
            Assert.AreEqual("indexed-term", storage.Get("/users/$index/lastname/tompson"));
        }

        [TestMethod]
        public void then_firstname_jane_indexed_row_for_rowkey_1_should_be_created()
        {
            Assert.AreEqual("/users/1", storage.Get("/users/$index/firstname/jane/1"));
        }

        [TestMethod]
        public void then_lastname_tompson_indexed_row_for_rowkey_1_should_be_created()
        {
            Assert.AreEqual("/users/1", storage.Get("/users/$index/lastname/tompson/1"));
        }

        [TestMethod]
        public void then_row_with_rowkey_1_should_be_created()
        {
            Assert.AreEqual("row", storage.Get("/users/1"));
        }

        [TestMethod]
        public void then_firstname_for_row_with_rowkey_1_should_be_jane()
        {
            Assert.AreEqual("jane", storage.Get("/users/1/$column/firstname"));
        }

        [TestMethod]
        public void then_lastname_for_row_with_rowkey_1_should_be_tompson()
        {
            Assert.AreEqual("tompson", storage.Get("/users/1/$column/lastname"));
        }
    }
}
