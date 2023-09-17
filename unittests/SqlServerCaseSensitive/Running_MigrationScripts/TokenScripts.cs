using NUnit.Framework;
using TestCommon.TestInfrastructure;

namespace SqlServerCaseSensitive.Running_MigrationScripts
{
    [TestFixture]
    [Category("SqlServerCaseSensitive")]
    public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts
    {
        protected override IGrateTestContext Context => GrateTestContext.SqlServerCaseSensitive;
    }
}
