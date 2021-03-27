using System;
using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public class Migrator
    {
        private readonly MooConfiguration _config;

        public Migrator(MooConfiguration config)
        {
            _config = config;
        }

        public async Task Migrate()
        {
            await Task.CompletedTask;
            Console.WriteLine("Moo.Migrate: config.Database is: " + _config.Database);
        }
    }
}