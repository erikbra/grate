using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace SqlServer.Running_MigrationScripts;

[TestFixture]
[Category("SqlServer")]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts: TestCommon.Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.SqlServer;
    protected override string ExpectedErrorMessageForInvalidSql => "Incorrect syntax near 'TOP'.";
}
