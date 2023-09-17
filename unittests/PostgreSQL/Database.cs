using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL;

[TestFixture]
[Category("PostgreSQL")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
