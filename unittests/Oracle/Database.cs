using NUnit.Framework;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace Oracle;

[TestFixture]
[Category("Oracle")]
public class Database: GenericDatabase
{
    protected override IGrateTestContext Context => Oracle.GrateTestContext.Oracle;
}
