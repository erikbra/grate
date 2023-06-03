using System.Collections.Generic;

namespace grate.Configuration;

public interface IFoldersConfiguration: IDictionary<string, MigrationsFolder?>
{
    MigrationsFolder? CreateDatabase { get; set; }
    MigrationsFolder? DropDatabase { get; set; }
}
