﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using grate.Configuration;
using grate.Migration;

namespace grate.Commands;

public static class CustomFoldersCommand
{
    public static IFoldersConfiguration Parse(string? arg)
    {
        if (IsFile(arg))
        {
            arg = File.Exists(arg) ? File.ReadAllText(arg) : "{}";
        }
        
        string? content = arg switch
        {
            { Length: 0 } => null,
            { } a when IsFile(a) => File.ReadAllText(a),
            _ => arg
        };

        return content switch
        {
            { } c when IsJson(c) => ParseCustomFoldersConfiguration(c),
            _ => KnownFolders.In(KnownFolderNamesArgument.Parse(arg))
        };
    }

    private static bool IsJson(string s) => s?.Trim()?.StartsWith("{") ?? false;

    private static IFoldersConfiguration ParseCustomFoldersConfiguration(string content)
    {
        var parsed = JsonSerializer.Deserialize<ParseableFolderConfiguration>(content, SerializerOptions);
        return (parsed) switch
        {
            null => FoldersConfiguration.Empty,
            { } => new FoldersConfiguration(parsed.MigrationsFolders),
        };
    }

    private static bool IsFile(string? s)
    {
        return s is not null && (File.Exists(s) || s.StartsWith("/")) ;
    }

    public static readonly JsonSerializerOptions SerializerOptions;

    static CustomFoldersCommand()
    {
        SerializerOptions = CreateSerializerOptions();
        SerializerOptions.Converters
            .Add(new ParseableMigrationsFolderJsonConverter());
    }

    private static JsonSerializerOptions CreateSerializerOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };
        options.Converters
            .Add(new JsonStringEnumConverter());

        return options;
    }

    private record ParseableFolderConfiguration
    {
        public Dictionary<string, ParseableMigrationsFolder> Folders { get; set; } = new();

        [JsonIgnore]
        public IDictionary<string, MigrationsFolder> MigrationsFolders =>
            Folders.ToDictionary(
                item => item.Key,
                item =>
                {
                    var (path, migrationType, connectionType, transactionHandling) = item.Value;
                    return new MigrationsFolder(
                        item.Key,
                        path ?? item.Key, 
                        migrationType,
                        connectionType, transactionHandling);
                });
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private record ParseableMigrationsFolder(
        string? Path,
        MigrationType Type,
        ConnectionType ConnectionType = ConnectionType.Default,
        TransactionHandling TransactionHandling = TransactionHandling.Default
    );

    /// <summary>
    /// Deserialized either a proper JSON representing a MigrationsFolder, or a string specifying
    /// just the MigrationType, in which case the rest of the values are filled out with default values.
    /// </summary>
    private class ParseableMigrationsFolderJsonConverter : JsonConverter<ParseableMigrationsFolder>
    {
        private static readonly JsonSerializerOptions OptionsWithoutMe = CreateSerializerOptions();

        static ParseableMigrationsFolderJsonConverter()
        {
        }

        public override ParseableMigrationsFolder? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            string stringValue = "";
            if (reader.TokenType == JsonTokenType.String)
            {
                stringValue = reader.GetString() ?? "";

                if (Enum.TryParse<MigrationType>(stringValue, true, out var migrationType))
                {
                    return new ParseableMigrationsFolder(null, migrationType);
                }
            }

            return JsonSerializer.Deserialize<ParseableMigrationsFolder>(ref reader, OptionsWithoutMe);
        }

        public override void Write(Utf8JsonWriter writer, ParseableMigrationsFolder value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, OptionsWithoutMe);
        }
    }
    
}
