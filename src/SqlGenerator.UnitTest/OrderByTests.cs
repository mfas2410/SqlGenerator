using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlGenerator.UnitTest.Model;

namespace SqlGenerator.UnitTest
{
    [TestClass]
    public class OrderByTests
    {
        private string SelectTable1 = $"[{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string SelectTable2 = $"[{nameof(Table2_1)}].[{nameof(Table2_1.Id)}],[{nameof(Table2_1)}].[{nameof(Table2_1.Table1Id)}],[{nameof(Table2_1)}].[{nameof(Table2_1.Name)}]";
        private string FromTable1 = $"[{nameof(Table1)}]";
        private string FromTable2 = $"[{nameof(Table2_1)}]";

        [TestMethod]
        public void OrderBySingleMember()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1} FROM {FromTable1} ORDER BY [{nameof(Table1)}].[{nameof(Table1.Name)}]",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().OrderBy(x => x.Name).ToString());
        }

        [TestMethod]
        public void OrderBySingleMemberAlias()
        {
            Table1 t1alias = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t1alias)}].[{nameof(Table1.Name)}],[{nameof(t1alias)}].[{nameof(Table1.IsOk)}] FROM {FromTable1} AS [{nameof(t1alias)}] ORDER BY [{nameof(t1alias)}].[{nameof(Table1.Name)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias).OrderBy(x => x.Name).ToString());
        }

        public void OrderBySingleUnary()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1} FROM {FromTable1} ORDER BY [{nameof(Table1)}].[{nameof(Table1.Id)}]",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().OrderBy(x => x.Id).ToString());
        }

        [TestMethod]
        public void OrderBySingleUnaryAlias()
        {
            Table1 t1alias = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t1alias)}].[{nameof(Table1.Name)}],[{nameof(t1alias)}].[{nameof(Table1.IsOk)}] FROM {FromTable1} AS [{nameof(t1alias)}] ORDER BY [{nameof(t1alias)}].[{nameof(Table1.Id)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias).OrderBy(x => x.Id).ToString());
        }

        [TestMethod]
        public void OrderByMultiple()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1} FROM {FromTable1} ORDER BY [{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}]",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().OrderBy(x => new { x.Id, x.Name }).ToString());
        }

        [TestMethod]
        public void OrderByMultipleAlias()
        {
            Table1 t1alias = null;
            Table2_1 t2alias = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t1alias)}].[{nameof(Table1.Name)}],[{nameof(t1alias)}].[{nameof(Table1.IsOk)}],[{nameof(t2alias)}].[{nameof(Table2_1.Id)}],[{nameof(t2alias)}].[{nameof(Table2_1.Table1Id)}],[{nameof(t2alias)}].[{nameof(Table2_1.Name)}] FROM {FromTable1} AS [{nameof(t1alias)}] {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} AS [{nameof(t2alias)}] ON [{nameof(t1alias)}].[{nameof(Table1.Id)}]=[{nameof(t2alias)}].[{nameof(Table2_1.Table1Id)}] ORDER BY [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t2alias)}].[{nameof(Table2_1.Name)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias).Join(() => t2alias, (t1, t2) => t1.Id == t2.Table1Id).OrderBy(() => new { t1alias.Id, t2alias.Name }).ToString());
        }
    }
}
