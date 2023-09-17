using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Everytime_scripts: Generic.Running_MigrationScripts.Everytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
