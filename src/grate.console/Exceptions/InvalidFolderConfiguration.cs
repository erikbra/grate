using System;
using System.Runtime.CompilerServices;

namespace grate.Exceptions;

public class InvalidFolderConfiguration : Exception
{
    public InvalidFolderConfiguration(
        string? folderConfiguration,
        string? propertyName) : base("Invalid property name: " + propertyName + ". Folder configuration: " + folderConfiguration)
    { }
}
