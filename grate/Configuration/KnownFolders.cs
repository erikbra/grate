namespace grate.Configuration;

public static class KnownFolders
{
    public static IFoldersConfiguration In(IKnownFolderNames? folderNames = null) => FoldersConfiguration.In(folderNames);

}
