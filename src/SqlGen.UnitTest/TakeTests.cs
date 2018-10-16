using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SqlGen.UnitTest.Model;

namespace SqlGen.UnitTest
{
    [TestClass]
    public class TakeTests
    {
        private string SelectTable = $"SELECT [{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string FromTable = $" FROM [{nameof(Table1)}]";

        [TestMethod]
        public void Take()
        {
            const int take = 1;
            Assert.AreEqual(
                $"{SelectTable}{FromTable} OFFSET {default(int)} ROWS FETCH NEXT {take} ROWS ONLY",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Take(take).ToString());
        }

        [TestMethod]
        public void TakeThrows()
        {
            Assert.ThrowsException<ArgumentException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Take(0));
        }
    }
}
