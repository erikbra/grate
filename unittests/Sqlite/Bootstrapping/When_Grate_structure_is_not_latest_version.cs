﻿using grate.Configuration;
using Sqlite.TestInfrastructure;
using TestCommon.TestInfrastructure;

namespace Sqlite.Bootstrapping;

[Collection(nameof(SqliteTestDatabase))]
// ReSharper disable once InconsistentNaming
// ReSharper disable once UnusedType.Global
public class When_Grate_structure_is_not_latest_version(SqliteGrateTestContext context, ITestOutputHelper testOutput)
    : TestCommon.Generic.Bootstrapping.When_Grate_structure_is_not_latest_version(context, testOutput)
{
    public override Task The_latest_version_is_applied(string versionTableName) => Task.CompletedTask;
}

