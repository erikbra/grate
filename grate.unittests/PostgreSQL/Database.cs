using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL;

[TestFixture]
[Category("PostgreSQL")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
