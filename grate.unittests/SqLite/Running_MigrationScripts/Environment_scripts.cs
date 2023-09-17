using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Environment_scripts: Generic.Running_MigrationScripts.Environment_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
