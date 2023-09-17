using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database: Generic.Running_MigrationScripts.Versioning_The_Database
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
