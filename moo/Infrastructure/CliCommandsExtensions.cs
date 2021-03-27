using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using moo.Commands;

namespace moo.Infrastructure
{
    public static class CliCommandCollectionExtensions
    {
        public static IServiceCollection AddCliCommands(this IServiceCollection services)
        {
            Type mooCommandType = typeof(MooCommand);
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