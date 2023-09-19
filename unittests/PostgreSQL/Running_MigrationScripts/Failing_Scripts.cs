using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Failing_Scripts: TestCommon.Generic.Running_MigrationScripts.Failing_Scripts
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;

    protected override string ExpectedErrorMessageForInvalidSql =>
        @"42703: column ""top"" does not exist

POSITION: 8";
}
