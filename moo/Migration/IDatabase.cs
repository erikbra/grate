using System.Threading.Tasks;
using moo.Configuration;

namespace moo.Migration
{
    public interface IDatabase
    {
        string? ServerName { get; }
        string? DatabaseName { get; set; }
        Task InitializeConnections(MooConfiguration configuration);
    }
}