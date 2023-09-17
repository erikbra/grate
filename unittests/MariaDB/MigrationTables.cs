using NUnit.Framework;
using TestCommon;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace MariaDB;

[TestFixture]
[Category("MariaDB")]
public class MigrationTables: GenericMigrationTables
{
    protected override IGrateTestContext Context => GrateTestContext.MariaDB;
}
