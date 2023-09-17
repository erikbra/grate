using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts: TestCommon.Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    protected override string ExpectedErrorMessageForInvalidSql => "SQLite Error 1: 'no such column: TOP'.";
}
