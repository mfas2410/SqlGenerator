using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SqlGen.UnitTest
{
    [TestClass]
    public class QueryBuilderTests
    {
        [TestMethod]
        public void QueryBuilderInvalidDialect()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new QueryBuilder((Dialect)int.MaxValue).ToString());
        }

        [TestMethod]
        public void QueryBuilderNoQuery()
        {
            Assert.ThrowsException<InvalidOperationException>(() => new QueryBuilder(Dialect.TSql2017).ToString());
        }
    }
}
