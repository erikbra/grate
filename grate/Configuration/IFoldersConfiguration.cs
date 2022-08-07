using System.Collections.Generic;

namespace grate.Configuration;

public interface IFoldersConfiguration: IReadOnlyDictionary<string, MigrationsFolder?>
{
}
