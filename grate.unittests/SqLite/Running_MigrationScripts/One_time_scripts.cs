using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
// ReSharper disable once InconsistentNaming
public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
