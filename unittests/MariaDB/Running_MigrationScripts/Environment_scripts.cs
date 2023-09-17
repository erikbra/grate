using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Environment_scripts: TestCommon.Generic.Running_MigrationScripts.Environment_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
