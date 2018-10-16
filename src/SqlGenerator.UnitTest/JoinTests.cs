using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlGenerator.UnitTest.Model;
using System;

namespace SqlGenerator.UnitTest
{
    [TestClass]
    public class JoinTests
    {
        private string SelectTable1 = $"[{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string SelectTable2 = $"[{nameof(Table2_1)}].[{nameof(Table2_1.Id)}],[{nameof(Table2_1)}].[{nameof(Table2_1.Table1Id)}],[{nameof(Table2_1)}].[{nameof(Table2_1.Name)}]";
        private string SelectTable3 = $"[{nameof(Table3_1)}].[{nameof(Table3_1.Id)}],[{nameof(Table3_1)}].[{nameof(Table3_1.Table1Id)}],[{nameof(Table3_1)}].[{nameof(Table3_1.Name)}]";
        private string SelectTable4 = $"[{nameof(Table4_3)}].[{nameof(Table4_3.Id)}],[{nameof(Table4_3)}].[{nameof(Table4_3.Table3Id)}],[{nameof(Table4_3)}].[{nameof(Table4_3.Name)}]";
        private string FromTable1 = $"[{nameof(Table1)}]";
        private string FromTable2 = $"[{nameof(Table2_1)}]";
        private string FromTable3 = $"[{nameof(Table3_1)}]";
        private string FromTable4 = $"[{nameof(Table4_3)}]";
        private string OnTable1Table2_1 = $"[{nameof(Table1)}].[{nameof(Table1.Id)}]=[{nameof(Table2_1)}].[{nameof(Table2_1.Table1Id)}]";
        private string OnTable1Table3_1 = $"[{nameof(Table1)}].[{nameof(Table1.Id)}]=[{nameof(Table3_1)}].[{nameof(Table3_1.Table1Id)}]";
        private string OnTable3_1Table4_3 = $"[{nameof(Table3_1)}].[{nameof(Table3_1.Id)}]=[{nameof(Table4_3)}].[{nameof(Table4_3.Table3Id)}]";

        [TestMethod]
        public void JoinAliasInner()
        {
            Table1 t1alias = null;
            Table2_1 t2Alias = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias)}].[{nameof(Table1.Id)}],[{nameof(t1alias)}].[{nameof(Table1.Name)}],[{nameof(t1alias)}].[{nameof(Table1.IsOk)}],[{nameof(t2Alias)}].[{nameof(Table2_1.Id)}],[{nameof(t2Alias)}].[{nameof(Table2_1.Table1Id)}],[{nameof(t2Alias)}].[{nameof(Table2_1.Name)}] FROM {FromTable1} AS [{nameof(t1alias)}] {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} AS [{nameof(t2Alias)}] ON [{nameof(t1alias)}].[{nameof(Table1.Id)}]=[{nameof(t2Alias)}].[{nameof(Table2_1.Table1Id)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias).Join(() => t2Alias, (t1, t2) => t1.Id == t2.Table1Id).ToString());
        }

        [TestMethod]
        public void JoinAliasInnerSelf()
        {
            Table1 t1alias1 = null;
            Table1 t1alias2 = null;
            Assert.AreEqual(
                $"SELECT [{nameof(t1alias1)}].[{nameof(Table1.Id)}],[{nameof(t1alias1)}].[{nameof(Table1.Name)}],[{nameof(t1alias1)}].[{nameof(Table1.IsOk)}],[{nameof(t1alias2)}].[{nameof(Table1.Id)}],[{nameof(t1alias2)}].[{nameof(Table1.Name)}],[{nameof(t1alias2)}].[{nameof(Table1.IsOk)}] FROM {FromTable1} AS [{nameof(t1alias1)}] {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable1} AS [{nameof(t1alias2)}] ON [{nameof(t1alias1)}].[{nameof(Table1.Id)}]=[{nameof(t1alias2)}].[{nameof(Table1.Id)}]",
                new QueryBuilder(Dialect.TSql2017).From(() => t1alias1).Join(() => t1alias2, (t1, t2) => t1.Id == t2.Id).ToString());
        }

        [TestMethod]
        public void JoinHierarchyInner()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable3},{SelectTable4} FROM {FromTable1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable3} ON {OnTable1Table3_1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable4} ON {OnTable3_1Table4_3}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table3_1>((t1, t2) => t1.Id == t2.Table1Id).Join<Table4_3>((t1, t2) => t1.Id == t2.Table3Id).ToString());
        }

        [TestMethod]
        public void JoinMultipleInner()
        {
            var query = new QueryBuilder(Dialect.TSql2017).From<Table1>();
            query.Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id);
            query.Join<Table3_1>((t1, t2) => t1.Id == t2.Table1Id);

            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2},{SelectTable3} FROM {FromTable1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} ON {OnTable1Table2_1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable3} ON {OnTable1Table3_1}",
                query.ToString());
        }

        [TestMethod]
        public void JoinSingleInner()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} ON {OnTable1Table2_1}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id).ToString());
        }

        [TestMethod]
        public void JoinSingleInnerMultipleWhere1()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} ON ({OnTable1Table2_1}) AND ([{nameof(Table2_1)}].[{nameof(Table2_1.Id)}]>2)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id && t2.Id > 2).ToString());
        }


        [TestMethod]
        public void JoinSingleInnerMultipleWhere2()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Inner).ToUpper()} JOIN {FromTable2} ON ({OnTable1Table2_1}) AND ([{nameof(Table2_1)}].[{nameof(Table2_1.Name)}] IS NOT NULL)",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id && t2.Name != null).ToString());
        }

        [TestMethod]
        public void JoinSingleFull()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Full).ToUpper()} JOIN {FromTable2} ON {OnTable1Table2_1}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id, JoinType.Full).ToString());
        }

        [TestMethod]
        public void JoinSingleLeft()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Left).ToUpper()} JOIN {FromTable2} ON {OnTable1Table2_1}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id, JoinType.Left).ToString());
        }

        [TestMethod]
        public void JoinSingleRight()
        {
            Assert.AreEqual(
                $"SELECT {SelectTable1},{SelectTable2} FROM {FromTable1} {nameof(JoinType.Right).ToUpper()} JOIN {FromTable2} ON {OnTable1Table2_1}",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>((t1, t2) => t1.Id == t2.Table1Id, JoinType.Right).ToString());
        }

        [TestMethod]
        public void JoinThrowsArgumentNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Join<Table2_1>(null));
        }
    }
}
