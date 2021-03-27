using System.CommandLine;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using moo.Commands;
using moo.Infrastructure;

namespace moo
{
    public static class Program
    {
        
        private static readonly ServiceProvider ServiceProvider = BuildServiceProvider();
        private static bool _verbose;

        public static async Task<int> Main(string[] args)
        {

            var rootCommand = Create<MigrateCommand>();
            rootCommand.Add(Verbose());
            
            //var rootCommand =new MigrateCommand()
            //{
                //Verbose(),
            //};
            
            rootCommand.Description = "The new moo - sql for the 20s";
            //rootCommand.Handler = CommandHandler.Create<bool>(SetVerbose);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        private static void SetVerbose(bool verbose) => _verbose = verbose;
        
        
        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddCliCommands();
            services.AddLogging(logging =>
                logging
                    .AddConsole()
                    .SetMinimumLevel(_verbose ? LogLevel.Trace : LogLevel.Information));
          
            return services.BuildServiceProvider();
        }
        
        private static Option<bool> Verbose() => new(new[] {"-v", "--verbose"}, "Verbose output");
        
        private static T Create<T>() where T: notnull => ServiceProvider.GetRequiredService<T>();
    }
}
