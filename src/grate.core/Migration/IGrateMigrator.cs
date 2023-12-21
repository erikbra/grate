namespace grate.Migration;
public interface IGrateMigrator
{
    /// <summary>
    /// Trigger grate migration with the given configuration
    /// </summary>
    /// <returns></returns>
    Task Migrate();
}
