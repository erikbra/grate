using NUnit.Framework;
using TestCommon;
using TestCommon.TestInfrastructure;

namespace Sqlite.Running_MigrationScripts;

[TestFixture]
[Category("Sqlite")]
public class TokenScripts : TestCommon.Generic.Running_MigrationScripts.TokenScripts
{
    protected override IGrateTestContext Context => GrateTestContext.Sqlite;
}
