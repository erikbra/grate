﻿using grate.unittests.TestInfrastructure;

namespace grate.unittests;

public static class GrateTestContext
{
    internal static readonly SqlServerGrateTestContext SqlServer = new();
    internal static readonly OracleGrateTestContext Oracle = new();
    internal static readonly PostgreSqlGrateTestContext PostgreSql = new();
    // ReSharper disable once InconsistentNaming
    internal static readonly MariaDbGrateTestContext MariaDB = new();
    internal static readonly SqliteGrateTestContext Sqlite = new();
}
