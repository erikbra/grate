using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle.Running_MigrationScripts
{
    [TestFixture]
    [Category("Oracle")]
    public class One_time_scripts: Generic.Running_MigrationScripts.One_time_scripts
    {
        protected override IGrateTestContext Context => GrateTestContext.Oracle;

        protected override string CreateView1 => base.CreateView1 + " FROM DUAL";
        protected override string CreateView2 => base.CreateView2 + " FROM DUAL";
    }
}
