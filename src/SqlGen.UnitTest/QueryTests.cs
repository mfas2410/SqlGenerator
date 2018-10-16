using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SqlGen.UnitTest.Model;

namespace SqlGen.UnitTest
{
    [TestClass]
    public class QueryTests
    {
        private string SelectTable = $"[{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string FromTable = $" FROM [{nameof(Table1)}]";

        [TestMethod]
        public void QueryAlias()
        {
            Table1 t1alias = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t1alias)}].[{nameof(Table1.Name)}],[{nameof(t1alias)}].[{nameof(Table1.IsOk)}]{FromTable} AS [{nameof(t1alias)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias).ToString());
        }

        [TestMethod]
        public void QueryAllProperties()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable}{FromTable}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().ToString());
        }

        [TestMethod]
        public void QueryDistinctAllProperties()
        {
            Assert.AreEqual(
                $"SELECT DISTINCT {SelectTable}{FromTable}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Distinct().ToString());
        }

        [TestMethod]
        public void QueryMultipleProperties()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable}{FromTable}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Select(x => new { x.Id, x.Name, x.IsOk }).ToString());
        }

        [TestMethod]
        public void QuerySingleProperty1()
        {
            Assert.AreEqual(
                $"SELECT [{nameof(Table1)}].[{nameof(Table1.Id)}]{FromTable}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Select(x => x.Id).ToString());
        }

        [TestMethod]
        public void QuerySingleProperty2()
        {
            Assert.AreEqual(
                $"SELECT [{nameof(Table1)}].[{nameof(Table1.Name)}]{FromTable}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Select(x => x.Name).ToString());
        }

        [TestMethod]
        public void QueryThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Select(x => x.Id == 1));
        }

        [TestMethod]
        public void QueryThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Select(null));
        }
    }
}
