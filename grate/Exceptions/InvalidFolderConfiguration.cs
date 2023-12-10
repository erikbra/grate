namespace grate.Exceptions;

public class InvalidFolderConfiguration : Exception
{
    public InvalidFolderConfiguration(
        string? folderConfiguration,
        string? propertyName) : base("Invalid property name: " + propertyName + ". Folder configuration: " + folderConfiguration)
    { }
}
