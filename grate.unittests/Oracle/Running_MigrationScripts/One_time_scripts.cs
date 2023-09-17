using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Oracle.Running_MigrationScripts;

[TestFixture]
[Category("Oracle")]
// ReSharper disable once InconsistentNaming
public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;

    protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
    protected override string CreateView2 => base.CreateView2 + " FROM DUAL";
}
