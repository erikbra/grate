using grate.Migration;

namespace grate.Configuration;

public record MigrationsFolder(
        string Name,
        string RelativePath,
        MigrationType Type,
        ConnectionType ConnectionType = ConnectionType.Default,
        TransactionHandling TransactionHandling = TransactionHandling.Default)
    : SubFolder(Name, RelativePath)
{
    public MigrationsFolder(
        string name,
        MigrationType type,
        ConnectionType connectionType = ConnectionType.Default,
        TransactionHandling transactionHandling = TransactionHandling.Default)
        : this(name, name, type, connectionType, transactionHandling)
    { }

}
