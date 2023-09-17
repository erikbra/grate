using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts: TestCommon.Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    protected override string ExpectedErrorMessageForInvalidSql => "Unknown column 'TOP' in 'field list'";
}
