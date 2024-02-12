namespace grate.Exceptions;

public class InvalidFolderConfiguration(
    string? folderConfiguration,
    string? propertyName)
    : Exception("Invalid property name: " + propertyName + ". Folder configuration: " + folderConfiguration);
