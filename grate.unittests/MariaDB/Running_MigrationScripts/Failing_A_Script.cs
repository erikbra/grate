using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Failing_A_Script: Generic.Running_MigrationScripts.Failing_A_Script
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
    protected override string ExpectedErrorMessageForInvalidSql => "Unknown column 'TOP' in 'field list'";
}
