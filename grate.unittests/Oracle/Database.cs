using NUnit.Framework;
using Unit_tests.TestInfrastructure;

namespace Unit_tests.Oracle;

[TestFixture]
[Category("Oracle")]
public class Database: Generic.GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.Oracle;
}
