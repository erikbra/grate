using System;
using System.Collections.Generic;
using System.Linq;

namespace grate.Infrastructure.Npgsql;

// These are just dummies, to be able to use the SqlQueryParser 100% as-is, without rewrites.
// This makes updating the class easier if it should change in the future.
using NpgsqlBatchCommand = DummyBatchCommand;
using NpgsqlCommand = DummyNpgsqlCommand;
using NpgsqlParameter = DummyNpgsqlParameter;
using NpgsqlParameterCollection = DummyNpgsqlParameterCollection;
using PlaceholderType = DummyPlaceholderType;
using ThrowHelper = DummyThrowHelper;


/// <summary>
/// Parses PostgreSQL text, using SqlQueryParser, adopted from the Npgsql code, to be as
/// compatible as we can.
/// </summary>
public static class NativeSqlQueryParser
{
    public static IEnumerable<string> Split(string sql)
    {
        IEnumerable<NpgsqlBatchCommand> batchCommands = ParseCommand(sql, Array.Empty<NpgsqlParameter>(), true);
        var statements = batchCommands
            .Select(b => b.FinalCommandText)
            .Cast<string>();
        return statements;
    }


    /// <summary>
    /// 
    ///  Method "stolen" from Unit-test in Npgsql: https://github.com/npgsql/npgsql/blob/main/test/Npgsql.Tests/SqlQueryParserTests.cs#L196
    /// 
    /// Copyright (c) 2002-2023, Npgsql
    /// 
    /// Permission to use, copy, modify, and distribute this software and its
    /// documentation for any purpose, without fee, and without a written agreement
    /// is hereby granted, provided that the above copyright notice and this
    /// paragraph and the following two paragraphs appear in all copies.
    /// 
    /// IN NO EVENT SHALL NPGSQL BE LIABLE TO ANY PARTY FOR DIRECT, INDIRECT,
    /// SPECIAL, INCIDENTAL, OR CONSEQUENTIAL DAMAGES, INCLUDING LOST PROFITS,
    /// ARISING OUT OF THE USE OF THIS SOFTWARE AND ITS DOCUMENTATION, EVEN IF
    /// Npgsql HAS BEEN ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    /// 
    /// NPGSQL SPECIFICALLY DISCLAIMS ANY WARRANTIES, INCLUDING, BUT NOT LIMITED
    /// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
    /// PURPOSE. THE SOFTWARE PROVIDED HEREUNDER IS ON AN "AS IS" BASIS, AND Npgsql
    /// HAS NO OBLIGATIONS TO PROVIDE MAINTENANCE, SUPPORT, UPDATES, ENHANCEMENTS,
    /// OR MODIFICATIONS.
    /// </summary>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="standardConformingStrings"></param>
    /// <returns></returns>
    private static List<NpgsqlBatchCommand> ParseCommand(string sql, NpgsqlParameter[] parameters, bool standardConformingStrings)
    {
        var cmd = new NpgsqlCommand(sql);
        cmd.Parameters.AddRange(parameters);
        var parser = new SqlQueryParser();
        parser.ParseRawQuery(cmd, standardConformingStrings);
        return cmd.InternalBatchCommands;
    }
}
