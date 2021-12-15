using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
public class Failing_Scripts: Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string ExpextedErrorMessageForInvalidSql =>
        @"ORA-00923: FROM keyword not found where expected";
}