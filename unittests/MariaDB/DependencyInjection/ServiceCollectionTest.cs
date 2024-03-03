﻿using MariaDB.TestInfrastructure;
using TestCommon.TestInfrastructure;
namespace MariaDB.DependencyInjection;

[Collection(nameof(MariaDbGrateTestContext))]
// ReSharper disable once UnusedType.Global
public class ServiceCollectionTest(MariaDbGrateTestContext context)
    : TestCommon.DependencyInjection.GrateServiceCollectionTest(context);
