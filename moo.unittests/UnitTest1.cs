using System;
using NUnit.Framework;

namespace moo.unittests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            var db = "Jallajalla";
            var pw = MooTestContext.SqlServer.AdminPassword;
            var port = MooTestContext.SqlServer.Port;

            var connectionString = $"Data Source=localhost,{port};Initial Catalog={db};User Id=sa;Password={pw}";
            TestContext.Progress.WriteLine("Connection string: " + connectionString);
        }
    }
}
