using NUnit.Framework;
using TestCommon;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[TestFixture]
[Category("PostgreSQL")]
public class DockerContainer: GenericDockerContainer
{
    protected override IGrateTestContext Context => GrateTestContext.PostgreSql;
}
