﻿using PostgreSQL.TestInfrastructure;
using TestCommon.Generic;
using TestCommon.TestInfrastructure;

namespace PostgreSQL;

[Collection(nameof(PostgresqlTestContainer))]
public class MigrationTables : GenericMigrationTables, IClassFixture<DependencyService>
{

    protected override IGrateTestContext Context { get; }

    protected ITestOutputHelper TestOutput { get; }

    public MigrationTables(PostgresqlTestContainer testContainer, DependencyService simpleService, ITestOutputHelper testOutput)
    {
        Context = new PostgreSqlGrateTestContext(simpleService.ServiceProvider, testContainer);
        TestOutput = testOutput;
    }

}
