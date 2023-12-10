using System.CommandLine;
using grate.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Infrastructure;

public static class CliCommandCollectionExtensions
{
    public static IServiceCollection AddCliCommands(this IServiceCollection services)
    {
        // Don't need this - might lead to issues when illink-ing, and we have only 1 of these
        // Type grateCommandType = typeof(GrateCommand);
        // Type commandType = typeof(Command);
        //
        // IEnumerable<Type> commands = grateCommandType
        //     .Assembly
        //     .GetExportedTypes()
        //     .Where(x => 
        //         x.Namespace == grateCommandType.Namespace && 
        //         !x.IsAbstract &&
        //         commandType.IsAssignableFrom(x));
        //
        // foreach (Type command in commands)
        // {
        //     services.AddSingleton(command, command);
        //     services.AddSingleton(commandType, command);
        // }

        services.AddSingleton<Command, MigrateCommand>();
        services.AddSingleton<MigrateCommand, MigrateCommand>();

        return services;
    }
}
