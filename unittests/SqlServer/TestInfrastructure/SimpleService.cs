﻿using grate;
using grate.SqlServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestCommon.TestInfrastructure;
namespace SqlServer.TestInfrastructure;
public class SimpleService
{
    public IServiceProvider ServiceProvider { get; }
    public SimpleService()
    {
        ServiceProvider = new ServiceCollection()
            .AddLogging(opt =>
            {
                opt.AddConsole();
                opt.SetMinimumLevel(TestConfig.GetLogLevel());
            })
            .AddGrate(cfg =>
            {
                cfg.UseSqlServer();
            })
            .AddSingleton<IDatabaseConnectionFactory, SqlServerConnectionFactory>()
            .BuildServiceProvider();
    }
}
