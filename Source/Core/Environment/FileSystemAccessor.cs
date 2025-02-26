namespace DotNetToolbox.Environment;

[ExcludeFromCodeCoverage(Justification = "Thin wrapper for OS functionality.")]
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global - Used for externally.
public class FileSystemAccessor
    : IHasDefault<FileSystemAccessor>
    , IFileSystemAccessor {
    public static FileSystemAccessor Default { get; } = new();

    public virtual char DirectorySeparatorChar => Path.DirectorySeparatorChar;
    public virtual string CombinePath(params string[] paths) => Path.Combine(paths);
    public virtual string[] GetFilePath(string filePath)
        => [.. Path.GetDirectoryName(filePath)?.Split(DirectorySeparatorChar) ?? [], GetFileName(filePath, true)];
    public virtual string GetFileName(string filePath, bool includeExtension = false)
        => includeExtension
               ? Path.GetFileName(filePath)
               : Path.GetFileNameWithoutExtension(filePath);
    public virtual string GetFileExtension(string filePath, bool includeDot = true)
        => includeDot
               ? Path.GetExtension(filePath)
               : Path.GetExtension(filePath).TrimStart('.');

    public virtual string CurrentFolder {
        get => Directory.GetCurrentDirectory();
        set => Directory.SetCurrentDirectory(value);
    }

    public virtual bool FolderExists(string folderPath)
        => Directory.Exists(folderPath);

    private static readonly EnumerationOptions _defaultEnumerationOptions = new();
    private static IEnumerable<string> GetFileSystemEntries(string baseFolder, SearchTarget target, string searchPattern = "*", EnumerationOptions? enumerationOptions = null) {
        baseFolder += IsNotNullOrWhiteSpace(baseFolder)[^1] is '\\' or '/' ? string.Empty : '\\';
        var list = target switch {
            SearchTarget.File => Directory.EnumerateFiles(baseFolder, searchPattern, enumerationOptions ?? _defaultEnumerationOptions),
            SearchTarget.Folder => Directory.EnumerateDirectories(baseFolder, searchPattern, enumerationOptions ?? _defaultEnumerationOptions),
            _ => Directory.EnumerateFileSystemEntries(baseFolder, searchPattern, enumerationOptions ?? _defaultEnumerationOptions),
        };
        return list.Select(f => f[baseFolder.Length..]);
    }

    private enum SearchTarget {
        Both,
        File,
        Folder,
    }

    public virtual IEnumerable<string> GetEntries(string baseFolder)
        => GetFileSystemEntries(baseFolder, SearchTarget.Both);

    public virtual IEnumerable<string> GetEntries(string baseFolder, string searchPattern)
        => GetFileSystemEntries(baseFolder, SearchTarget.Both, searchPattern);
    public virtual IEnumerable<string> GetEntries(string baseFolder, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.Both, enumerationOptions: enumerationOptions);
    public virtual IEnumerable<string> GetEntries(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.Both, searchPattern, enumerationOptions);
    public virtual IEnumerable<string> GetFiles(string baseFolder)
        => GetFileSystemEntries(baseFolder, SearchTarget.File);
    public virtual IEnumerable<string> GetFiles(string baseFolder, string searchPattern)
        => GetFileSystemEntries(baseFolder, SearchTarget.File, searchPattern);
    public virtual IEnumerable<string> GetFiles(string baseFolder, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.File, enumerationOptions: enumerationOptions);
    public virtual IEnumerable<string> GetFiles(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.File, searchPattern, enumerationOptions);
    public virtual IEnumerable<string> GetFolders(string baseFolder)
        => GetFileSystemEntries(baseFolder, SearchTarget.Folder);
    public virtual IEnumerable<string> GetFolders(string baseFolder, string searchPattern)
        => GetFileSystemEntries(baseFolder, SearchTarget.Folder, searchPattern);
    public virtual IEnumerable<string> GetFolders(string baseFolder, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.Folder, enumerationOptions: enumerationOptions);
    public virtual IEnumerable<string> GetFolders(string baseFolder, string searchPattern, EnumerationOptions enumerationOptions)
        => GetFileSystemEntries(baseFolder, SearchTarget.Folder, searchPattern, enumerationOptions);

    public virtual char FolderSeparator => Path.DirectorySeparatorChar;
    public virtual void CreateFolder(string folderPath)
        => Directory.CreateDirectory(folderPath);
    public virtual void DeleteFolder(string folderPath, bool includeAllContent = false)
        => Directory.Delete(folderPath, includeAllContent);

    public virtual bool FileExists(string filePath) => File.Exists(filePath);
    public virtual void MoveFile(string sourcePath, string targetPath, bool overwrite = false) => File.Move(sourcePath, targetPath, overwrite);
    public virtual void CopyFile(string sourcePath, string targetPath, bool overwrite = false) => File.Copy(sourcePath, targetPath, overwrite);
    public virtual void DeleteFile(string sourcePath) => File.Delete(sourcePath);

    public virtual Stream OpenFileAsReadOnly(string filePath, bool blockExternalAccess = true) => new FileStream(filePath, FileMode.Open, FileAccess.Read, blockExternalAccess ? FileShare.None : FileShare.ReadWrite);
    public virtual Stream OpenOrCreateFile(string filePath, bool blockExternalAccess = true) => new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, blockExternalAccess ? FileShare.None : FileShare.Read);
    public virtual Stream CreateNewOrOverwriteFile(string filePath, bool blockExternalAccess = true) => new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, blockExternalAccess ? FileShare.None : FileShare.Read);
}
