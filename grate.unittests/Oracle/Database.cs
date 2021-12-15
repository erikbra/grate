using grate.unittests.TestInfrastructure;
using NUnit.Framework;

namespace grate.unittests.Oracle;

[TestFixture]
[Category("Oracle")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}