namespace Services.FileManager
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IFileManager
    {
        bool DirectoryExists(FilePathInfo Path);
        Task<bool> DirectoryExistsAsync(FilePathInfo Path);

        void CreateDirectory(FilePathInfo Path);

        bool FileExists(FilePathInfo Path);
        Task<bool> FileExistsAsync(FilePathInfo Path);

        void SaveFile(Stream Stream, FilePathInfo Path);
        Task SaveFileAsync(Stream Stream, FilePathInfo Path);

        Stream OpenFile(FilePathInfo Path, bool CreateFile = false);
        Task<Stream> OpenFileAsync(FilePathInfo Path, bool CreateFile = false);

        bool DeleteFile(FilePathInfo Path);
        Task<bool> DeleteFileAsync(FilePathInfo Path);
    }
}
