using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace MariaDB.Running_MigrationScripts;

[TestFixture]
[Category("MariaDB")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: TestCommon.Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
