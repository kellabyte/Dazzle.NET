using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dazzle.Storage;

namespace Dazzle.Database.Tests
{
    [TestClass]
    public class when_selecting_firstname_where_firstname_equals_jane : with_database_having_2_rows
    {
        private QueryResult results;

        public override void When()
        {
            results = db.ExecuteQuery("select firstname from users where firstname = 'jane'");
        }

        [TestMethod]
        public void then_row_count_should_be_1()
        {
            Assert.AreEqual(1, results.Rows.Count);
        }

        [TestMethod]
        public void then_column_count_for_row_1_should_be_2()
        {
            Assert.AreEqual(2, results.Rows[0].Columns.Count);
        }

        [TestMethod]
        public void then_row_key_should_be_1()
        {
            Assert.AreEqual("1", results.Rows[0].RowKey);
        }

        [TestMethod]
        public void then_key_column_should_be_1()
        {
            Assert.AreEqual("1", results.Rows[0].Columns["key"]);
        }

        [TestMethod]
        public void then_firstname_column_should_be_jane()
        {
            Assert.AreEqual("jane", results.Rows[0].Columns["firstname"]);
        }
    }
}
