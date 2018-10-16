using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SqlGenerator.UnitTest.Model;

namespace SqlGenerator.UnitTest
{
    [TestClass]
    public class SkipTests
    {
        private string SelectTable = $"SELECT [{nameof(Table1)}].[{nameof(Table1.Id)}],[{nameof(Table1)}].[{nameof(Table1.Name)}],[{nameof(Table1)}].[{nameof(Table1.IsOk)}]";
        private string FromTable = $" FROM [{nameof(Table1)}]";

        [TestMethod]
        public void Skip()
        {
            const int skip = 1;
            Assert.AreEqual(
                $"{SelectTable}{FromTable} OFFSET {skip} ROWS FETCH NEXT {int.MaxValue} ROWS ONLY",
                new QueryBuilder(Dialect.TSql2017).From<Table1>().Skip(skip).ToString());
        }

        [TestMethod]
        public void SkipThrows()
        {
            Assert.ThrowsException<ArgumentException>(() => new QueryBuilder(Dialect.TSql2017).From<Table1>().Skip(0));
        }
    }
}
