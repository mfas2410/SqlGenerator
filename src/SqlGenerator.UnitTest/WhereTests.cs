using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlGenerator.UnitTest.Model;
using System;

namespace SqlGenerator.UnitTest
{
    [TestClass]
    public class WhereTests
    {
        private string SelectTable = $"SELECT [{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string FromTable = $" FROM [{nameof(Table1)}]";
        private string WhereId = $" WHERE ([{nameof(Table1)}].[{nameof(Table1.Id)}]";
        private string WhereName = $" WHERE ([{nameof(Table1)}].[{nameof(Table1.Name)}]";

        [TestMethod]
        public void WhereAnd()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable} WHERE (([{nameof(Table1)}].[{nameof(Table1.Id)}]<>0) AND ([{nameof(Table1)}].[{nameof(Table1.Name)}] IS NOT NULL))",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id != 0 & x.Name != null).ToString());
        }

        [TestMethod]
        public void WhereAndAlso()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable} WHERE (([{nameof(Table1)}].[{nameof(Table1.Id)}]<>0) AND ([{nameof(Table1)}].[{nameof(Table1.Name)}] IS NOT NULL))",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id != 0 && x.Name != null).ToString());
        }

        [TestMethod]
        public void WhereEquals()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}=0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id == 0).ToString());
        }

        [TestMethod]
        public void WhereGreaterThan()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}>0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id > 0).ToString());
        }

        [TestMethod]
        public void WhereGreaterThanOrEquals()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}>=0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id >= 0).ToString());
        }

        [TestMethod]
        public void WhereIsNull()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereName} IS NULL)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Name == null).ToString());
        }

        [TestMethod]
        public void WhereIsNotNull()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereName} IS NOT NULL)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Name != null).ToString());
        }

        [TestMethod]
        public void WhereLessThan()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}<0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id < 0).ToString());
        }

        [TestMethod]
        public void WhereLessThanOrEquals()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}<=0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id <= 0).ToString());
        }

        [TestMethod]
        public void WhereNot()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable} WHERE (NOT (([Table1].[Id]=0)) AND NOT (([Table1].[Name]='Test')))",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => !(x.Id == 0) && !(x.Name == "Test")).ToString());
        }

        [TestMethod]
        public void WhereNotEquals()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereId}<>0)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id != 0).ToString());
        }

        [TestMethod]
        public void WhereOr()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable} WHERE (([{nameof(Table1)}].[{nameof(Table1.Id)}]<>0) OR ([{nameof(Table1)}].[{nameof(Table1.Name)}] IS NOT NULL))",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id != 0 | x.Name != null).ToString());
        }

        [TestMethod]
        public void WhereOrElse()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable} WHERE (([{nameof(Table1)}].[{nameof(Table1.Id)}]<>0) OR ([{nameof(Table1)}].[{nameof(Table1.Name)}] IS NOT NULL))",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Id != 0 || x.Name != null).ToString());
        }

        [TestMethod]
        public void WhereText()
        {
            Assert.AreEqual(
                $"{SelectTable}{FromTable}{WhereName}='Test')",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(x => x.Name == "Test").ToString());
        }

        [TestMethod]
        public void WhereThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Where(null));
        }
    }
}
