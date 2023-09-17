using MariaDB.TestInfrastructure;

namespace MariaDB;

public static class GrateTestContext
{
    // ReSharper disable once InconsistentNaming
    internal static readonly MariaDbGrateTestContext MariaDB = new();
}
