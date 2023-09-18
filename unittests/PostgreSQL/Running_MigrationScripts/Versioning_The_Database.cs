using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database: TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
