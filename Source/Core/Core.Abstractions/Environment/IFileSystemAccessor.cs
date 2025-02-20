namespace DotNetToolbox.Environment;

public interface IFileSystemAccessor {
    char DirectorySeparatorChar { get; }

    string CombinePath(params string[] paths);
    void CopyFile(string sourcePath, string targetPath, bool overwrite = false);
    void CreateFolder(string folderPath);
    Stream CreateNewOrOverwriteFile(string filePath, bool blockExternalAccess = true);
    void DeleteFile(string sourcePath);
    void DeleteFolder(string folderPath, bool includeAllContent = false);
    bool FileExists(string filePath);
    string[] GetFilePath(string filePath);
    string GetFileName(string filePath, bool includeExtension = false);
    string GetFileExtension(string filePath, bool includeDot = true);
    char FolderSeparator { get; }
    string CurrentFolder { get; set; }
    bool FolderExists(string folderPath);
    IEnumerable<string> GetEntries(string baseFolder);
    IEnumerable<string> GetEntries(string baseFolder, string searchPattern);
    IEnumerable<string> GetEntries(string baseFolder, EnumerationOptions enumerationOptions);
    IEnumerable<string> GetEntries(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions);
    IEnumerable<string> GetFiles(string baseFolder);
    IEnumerable<string> GetFiles(string baseFolder, string searchPattern);
    IEnumerable<string> GetFiles(string baseFolder, EnumerationOptions enumerationOptions);
    IEnumerable<string> GetFiles(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions);
    IEnumerable<string> GetFolders(string baseFolder);
    IEnumerable<string> GetFolders(string baseFolder, string searchPattern);
    IEnumerable<string> GetFolders(string baseFolder, EnumerationOptions enumerationOptions);
    IEnumerable<string> GetFolders(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions);
    void MoveFile(string sourcePath, string targetPath, bool overwrite = false);
    Stream OpenFileAsReadOnly(string filePath, bool blockExternalAccess = true);
    Stream OpenOrCreateFile(string filePath, bool blockExternalAccess = true);
}
