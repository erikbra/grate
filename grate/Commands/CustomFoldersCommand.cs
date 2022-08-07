using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using grate.Configuration;
using grate.Exceptions;
using grate.Migration;

namespace grate.Commands;

public static class CustomFoldersCommand
{
    public static IFoldersConfiguration Parse(string? arg)
    {
        if (IsFile(arg))
        {
            arg = File.Exists(arg) ? File.ReadAllText(arg) : "";
            
            // Makes more sense to supply an empty config than the default if you actually supply a file,
            // but that file is either non-existant or empty
            if (string.IsNullOrEmpty(arg))
            {
                return FoldersConfiguration.Empty;
            }
        }
        
        string? content = arg switch
        {
            { Length: 0 } => null,
            { } a when IsFile(a) => File.ReadAllText(a),
            _ => arg
        };

        return content switch
        {
            //{ } c when IsJson(c) => ParseCustomFoldersConfiguration(c),
            { } c => ParseNewCustomFoldersConfiguration(c),
            _ => FoldersConfiguration.Default()
            //{ } c when IsNewCustomFolderFormat(c) => ParseNewCustomFoldersConfiguration(c),
            //_ => FoldersConfiguration.Default(KnownFolderNamesArgument.Parse(arg))
        };
    }

    private static IFoldersConfiguration ParseNewCustomFoldersConfiguration(string s)
    {
        // Combine lines into a semicolon-separated string, if there were multiple lines
        var lines = (s.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        var oneLine = string.Join(';', lines);
        var tokens = oneLine.Split(';', StringSplitOptions.RemoveEmptyEntries);

        IEnumerable<(string key, string config)> configs = tokens.Select(token => SplitInTwo(token, '=')).ToArray();

        // Check to see if any of the folders specified was one of the default ones.
        // If it was, assume that the intent is so have the default folder configuration, but just
        // adjust some of the folders.
        // 
        // If none of the folders are any of the default folders, we assume the intent is a totally 
        // customised folder configuration.
        IFoldersConfiguration foldersConfiguration =
            configs.Select(c => c.key).Any(key => KnownFolderKeys.Keys.Contains(key, StringComparer.InvariantCultureIgnoreCase))
                ? FoldersConfiguration.Default()
                : FoldersConfiguration.Empty;

        // Loop through all the config items, and apply them
        // 1) To the existing default folder with that name
        // 2) To a new folder, if there is no default folder with that name
        foreach ((string key, string config) in configs)
        {
            MigrationsFolder folder;
            var existingKey =
                foldersConfiguration.Keys.SingleOrDefault(k =>
                    key.Equals(k, StringComparison.InvariantCultureIgnoreCase));
            if (existingKey is not null)
            {
                folder = foldersConfiguration[existingKey]!;
            }
            else
            {
                folder = new MigrationsFolder(key);
                foldersConfiguration[key] = folder;
            }
            ApplyConfig(folder, config);
        }
        
        return foldersConfiguration;
    }

    /// <summary>
    /// Apply folder config from a string to an existing Migrations folder.
    /// </summary>
    /// <param name="folder">The folder to apply to</param>
    /// <param name="folderConfig">A string that describes the configuration, or short form with either
    /// only the folder name, or only the migration type</param>
    /// <exception cref="InvalidFolderConfiguration"></exception>
    /// <example>relativePath:myCustomFolder,type:Once,connectionType:Admin</example>
    /// <example>Once</example>
    /// <example>AnyTime</example>
    private static void ApplyConfig(MigrationsFolder folder, string folderConfig)
    {
        // First, handle any special 'short form' types:
        if (!folderConfig.Contains(':'))
        {
            if (Enum.TryParse(folderConfig, true, out MigrationType result))
            {
                var (setter, _) = GetProperty(nameof(MigrationsFolder.Type));
                setter!.Invoke(folder, new object?[] { result });
            }
            else
            {
                var (setter, _) = GetProperty(nameof(MigrationsFolder.RelativePath));
                setter!.Invoke(folder, new object?[] { folderConfig });
            }

            return;
        }
        

        var tokens = folderConfig.Split(',');
        foreach (var token in tokens)
        {
            var keyAndValue = token.Split(':', 2);
            var (key, value) = (keyAndValue.First(), keyAndValue.Last());

            var (setter, propertyType) = GetProperty(key);

            if (setter is null)
            {
                throw new InvalidFolderConfiguration(folderConfig, key);
            }

            var parsed = (propertyType?.IsEnum ?? false) 
                            ? Enum.Parse(propertyType, value) 
                            : value;

            setter.Invoke(folder, new[] { parsed });
        }
    }

    static readonly Type FolderType = typeof(MigrationsFolder);

    private static (MethodInfo? setter, Type? propertyType) GetProperty(string propertyName)
    {
        var property =
            FolderType.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        var setter = property?.GetSetMethod(true);
        var propertyType = property?.PropertyType;

        return (setter, propertyType);
    }

    private static (string key, string value) SplitInTwo(string s, char separator)
    {
            var keyAndValue = s.Split(separator, 2);
            var (key, value) = (keyAndValue.First(), keyAndValue.Last());
            return (key, value);
    }

    private static bool IsJson(string s) => s?.Trim()?.StartsWith("{") ?? false;
    private static bool IsNewCustomFolderFormat(string s) => s?.Trim()?.Contains(";") ?? false;
    
    

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

    private class ParseableFolderConfiguration: Dictionary<string, ParseableMigrationsFolder> 
    {
        [JsonIgnore]
        public IDictionary<string, MigrationsFolder> MigrationsFolders =>
            this.ToDictionary(
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
