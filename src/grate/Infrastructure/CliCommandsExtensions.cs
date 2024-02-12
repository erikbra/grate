using System.CommandLine;
using grate.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Infrastructure;

internal static class CliCommandCollectionExtensions
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        services.AddSingleton<Command, MigrateCommand>();
        services.AddSingleton<MigrateCommand, MigrateCommand>();

        return services;
    }
}
