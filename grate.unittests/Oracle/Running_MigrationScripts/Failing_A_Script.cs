using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class Failing_A_Script: Generic.Running_MigrationScripts.Failing_A_Script
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string ExpectedErrorMessageForInvalidSql =>
        @"ORA-00923: FROM keyword not found where expected";
}
