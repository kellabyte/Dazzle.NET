using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dazzle.Database.Tests
{
    [TestClass]
    public class when_selecting_firstname_and_lastname_where_firstname_equals_jane : with_database_having_2_rows
    {
        private QueryResult results;

        public override void When()
        {
            this.results = db.ExecuteQuery("select firstname, lastname from users where firstname = 'jane'");
        }

        [TestMethod]
        public void then_row_count_should_be_1()
        {
            Assert.AreEqual(1, this.results.Rows.Count);
        }

        [TestMethod]
        public void then_column_count_for_row_1_should_be_2()
        {
            Assert.AreEqual(3, this.results.Rows[0].Columns.Count);
        }

        [TestMethod]
        public void then_row_key_should_be_1()
        {
            Assert.AreEqual("1", this.results.Rows[0].RowKey);
        }

        [TestMethod]
        public void then_key_column_should_be_1()
        {
            Assert.AreEqual("1", this.results.Rows[0].Columns["key"]);
        }

        [TestMethod]
        public void then_firstname_column_should_be_jane()
        {
            Assert.AreEqual("jane", this.results.Rows[0].Columns["firstname"]);
        }

        [TestMethod]
        public void then_lastname_column_should_be_tompson()
        {
            Assert.AreEqual("tompson", this.results.Rows[0].Columns["lastname"]);
        }
    }
}