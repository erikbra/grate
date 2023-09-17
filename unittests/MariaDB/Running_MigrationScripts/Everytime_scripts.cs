using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Everytime_scripts: TestCommon.Generic.Running_MigrationScripts.Everytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
