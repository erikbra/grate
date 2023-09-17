using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Versioning_The_Database: TestCommon.Generic.Running_MigrationScripts.Versioning_The_Database
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
