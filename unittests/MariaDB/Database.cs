using NUnit.Framework;
using TestCommon;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[TestFixture]
[Category("MariaDB")]
public class Database: GenericDatabase
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
        
}
