using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class Failing_A_Script: Generic.Running_MigrationScripts.Failing_A_Script
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
    protected override string ExpectedErrorMessageForInvalidSql => "SQLite Error 1: 'no such column: TOP'.";
}
