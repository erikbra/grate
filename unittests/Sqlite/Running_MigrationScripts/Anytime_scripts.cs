using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: TestCommon.Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
