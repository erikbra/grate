namespace grate.Infrastructure.Npgsql;

public class DummyNpgsqlParameter
{
    public string ParameterName { get; set; }
    public bool IsInputDirection => true;
}
