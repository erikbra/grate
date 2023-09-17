using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts: Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string ExpectedErrorMessageForInvalidSql =>
        @"ORA-00923: FROM keyword not found where expected";
}
