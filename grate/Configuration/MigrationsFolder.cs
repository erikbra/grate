using grate.Migration;

namespace grate.Configuration;

public record MigrationsFolder(
        string Name,
        string RelativePath,
        MigrationType Type = MigrationType.Once,
        ConnectionType ConnectionType = ConnectionType.Default,
        TransactionHandling TransactionHandling = TransactionHandling.Default)
{
    public MigrationsFolder(
        string name,
        MigrationType type = MigrationType.Once,
        ConnectionType connectionType = ConnectionType.Default,
        TransactionHandling transactionHandling = TransactionHandling.Default)
        : this(name, name, type, connectionType, transactionHandling)
    { }

}
