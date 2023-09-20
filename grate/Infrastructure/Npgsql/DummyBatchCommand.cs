using Npgsql;

namespace grate.Infrastructure.Npgsql;

/// <summary>
/// Simple, minimal dummy implementation of the Npgsql BatchCommand,
/// only to support the methods needed for parsing/splitting
/// </summary>
public class DummyBatchCommand
{
    public string? FinalCommandText { get; set; }
    public string CommandText { get; set; }
    public DummyNpgsqlParameterCollection Parameters { get; set; }
    public DummyNpgsqlParameterCollection PositionalParameters { get; set; }

    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}
