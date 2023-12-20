using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Npgsql;

namespace grate.Infrastructure.Npgsql;

/// <summary>
/// Parses/splits PostgreSQL statements into batches, using
/// the internal Npgsql.SqlQueryParser via heavy use of reflection. 
/// </summary>
public static class ReflectionNpgsqlQueryParser
{
    // Heavy use of reflection here...
    private static readonly MethodInfo ParseRawQuery;
    private static readonly PropertyInfo InternalBatchCommands;
    private static readonly PropertyInfo State;
    private static readonly PropertyInfo IsCached;
    private static readonly PropertyInfo FinalCommandText;
    private static ConstructorInfo Constructor { get; }

#pragma warning disable CA1810
    // The code that's violating the rule is on this line.
    static ReflectionNpgsqlQueryParser()
    {
        var sqlQueryParserType = Type.GetType("Npgsql.SqlQueryParser, Npgsql")!;
        Constructor = sqlQueryParserType.GetConstructor(
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null, Type.EmptyTypes, null)!;

        var npgsqlCommand = typeof(NpgsqlCommand);
        var npgsqlBatchCommand = typeof(NpgsqlBatchCommand);

        ParseRawQuery = sqlQueryParserType.GetMethod("ParseRawQuery", BindingFlags.Instance | BindingFlags.NonPublic, new[] { npgsqlCommand, typeof(bool), typeof(bool) })!;

        InternalBatchCommands = npgsqlCommand.GetProperty("InternalBatchCommands", BindingFlags.Instance | BindingFlags.NonPublic)!;
        State = npgsqlCommand.GetProperty("State", BindingFlags.Instance | BindingFlags.NonPublic)!;
        IsCached = npgsqlCommand.GetProperty("IsCacheable", BindingFlags.Instance | BindingFlags.NonPublic)!;
        FinalCommandText = npgsqlBatchCommand.GetProperty("FinalCommandText", BindingFlags.Instance | BindingFlags.NonPublic)!;
    }
#pragma warning restore CA1810
    public static IEnumerable<string> Split(string sql)
    {
        var cmd = new NpgsqlCommand(sql);
        State.SetMethod!.Invoke(cmd, new object[] { 0 });
        IsCached.SetMethod!.Invoke(cmd, new object[] { false });
        cmd.CommandText = sql;

        var sqlQueryParser = Constructor.Invoke(Array.Empty<object>());
        ParseRawQuery.Invoke(sqlQueryParser, new object?[] { cmd, true, false });

        List<NpgsqlBatchCommand> batchCommands =
            (List<NpgsqlBatchCommand>)InternalBatchCommands.GetMethod!.Invoke(cmd, Array.Empty<object>())!;
        var statements = batchCommands
            .Select(b => FinalCommandText.GetMethod!.Invoke(b, Array.Empty<object>()))
            .Cast<string>();
        return statements;
    }

}
