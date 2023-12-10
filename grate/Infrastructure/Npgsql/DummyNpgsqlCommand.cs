namespace grate.Infrastructure.Npgsql;

public class DummyNpgsqlCommand
{
    public DummyNpgsqlCommand(string sql)
    {
        CommandText = sql;
    }

    public string CommandText { get; set; }
    public DummyNpgsqlParameterCollection Parameters { get; } = new();
    public List<DummyBatchCommand> InternalBatchCommands { get; } = new();
}
