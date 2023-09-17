using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Everytime_scripts: TestCommon.Generic.Running_MigrationScripts.Everytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
