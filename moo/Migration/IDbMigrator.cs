using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public interface IDbMigrator
    {
        Task InitializeConnections();
        IDatabase? Database { get; }
        MooConfiguration Configuration { get; set; }
        void ApplyConfig(MooConfiguration config);
    }
}