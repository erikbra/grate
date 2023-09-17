using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace TestCommon.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
