using System;
using System.Threading.Tasks;
using moo.Configuration;
using moo.Infrastructure;

namespace moo.Migration
{
    public class DbMigrator: IDbMigrator
    {
        private readonly IFactory _factory;

        public DbMigrator(IFactory factory)
        {
            _factory = factory;
            Configuration = MooConfiguration.Default;
        }
        
        public async Task InitializeConnections()
        {
            await Database?.InitializeConnections(Configuration)!;
        }

        public IDatabase Database { get; set; } = null!;

        public void ApplyConfig(MooConfiguration config)
        {
            this.Configuration = config;
            Database = _factory.GetService<DatabaseType, IDatabase>(config.DatabaseType);
        }

        public MooConfiguration Configuration { get; set; }
    }
}