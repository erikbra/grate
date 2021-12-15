using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle;

[TestFixture]
[Category("Oracle")]
public class MigrationTables: Generic.GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}