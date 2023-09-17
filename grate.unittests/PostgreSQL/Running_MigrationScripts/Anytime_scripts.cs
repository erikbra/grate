using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
