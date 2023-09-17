using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class Environment_scripts: TestCommon.Generic.Running_MigrationScripts.Environment_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
