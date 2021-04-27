using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using grate.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace grate.Infrastructure
{
    public static class CliCommandCollectionExtensions
    {
        public static IServiceCollection AddCliCommands(this IServiceCollection services)
        {
            Type mooCommandType = typeof(GrateCommand);
            Type commandType = typeof(Command);

            IEnumerable<Type> commands = mooCommandType
                .Assembly
                .GetExportedTypes()
                .Where(x => 
                    x.Namespace == mooCommandType.Namespace && 
                    !x.IsAbstract &&
                    commandType.IsAssignableFrom(x));

            foreach (Type command in commands)
            {
                services.AddSingleton(command, command);
                services.AddSingleton(commandType, command);
            }

            return services;
        }
    }
}