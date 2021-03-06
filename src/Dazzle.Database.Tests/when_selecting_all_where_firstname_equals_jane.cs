﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dazzle.Database.Tests
{
    [TestClass]
    public class when_selecting_all_where_firstname_equals_jane : with_database_having_2_rows
    {
        private QueryResult results;

        public override void When()
        {
            results = db.ExecuteQuery("select * from users where firstname = 'jane'");
        }

        [TestMethod]
        public void then_row_count_should_be_1()
        {
            Assert.AreEqual(1, results.Rows.Count);
        }

        [TestMethod]
        public void then_column_count_for_row_1_should_be_2()
        {
            Assert.AreEqual(3, results.Rows[0].Columns.Count);
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

        [TestMethod]
        public void then_lastname_column_should_be_thompson()
        {
            Assert.AreEqual("tompson", results.Rows[0].Columns["lastname"]);
        }
    }
}
