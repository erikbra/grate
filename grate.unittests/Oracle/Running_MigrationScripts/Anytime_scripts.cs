using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class Anytime_scripts: Generic.Running_MigrationScripts.Anytime_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
