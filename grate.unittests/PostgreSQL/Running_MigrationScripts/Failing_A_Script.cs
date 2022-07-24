using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.PostgreSQL.Running_MigrationScripts;

[TestFixture]
[Category("PostgreSQL")]
// ReSharper disable once InconsistentNaming
public class Failing_A_Script: Generic.Running_MigrationScripts.Failing_A_Script
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;

    protected override string ExpectedErrorMessageForInvalidSql =>
        @"42703: column ""top"" does not exist

POSITION: 8";
}
