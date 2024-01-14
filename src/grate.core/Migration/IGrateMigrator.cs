namespace grate.Migration;
public interface IGrateMigrator : IAsyncDisposable
{
    /// <summary>
    /// Trigger grate migration with the given configuration
    /// </summary>
    /// <returns></returns>
    Task Migrate();
    IDbMigrator DbMigrator { get; }
}
