namespace grate.Infrastructure.Npgsql;

public class DummyNpgsqlParameter
{
    public string ParameterName { get; set; } = null!;
    public bool IsInputDirection => true;
}
