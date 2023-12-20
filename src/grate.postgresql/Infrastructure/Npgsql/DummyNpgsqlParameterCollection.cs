using System.Collections.Generic;

namespace grate.Infrastructure.Npgsql;

public class DummyNpgsqlParameterCollection : Dictionary<string, DummyNpgsqlParameter>
{
    public void Add(DummyNpgsqlParameter parameter) => Add(parameter.ParameterName, parameter);
    public DummyPlaceholderType PlaceholderType => DummyPlaceholderType.Whatever;

    public void AddRange(DummyNpgsqlParameter[] parameters)
    {
        foreach (var parameter in parameters)
        {
            Add(parameter.ParameterName, parameter);
        }
    }
}
