namespace Services.FileManager
{
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Options;
  using System;
  using System.IO;
  using System.Threading.Tasks;

  /// <summary>
  /// Implements a file manager using local files and directories, all based on a common root directory. 
  /// If this is used within an ASP.NET application:
  ///     - If app setting RootPath is not configured, by default the root directory of the site will be the root path
  ///     - If RootPath is specified, it will be used from appsettings.json
  ///     - If the path begins with a \\ the path will be combined with the webroot path
  /// Outside of a web application:
  ///     - If app setting RootPath is not configured, by default the root path will be blank (i.e. working path of the 
  ///       executable)
  ///     - If RootPath is specified, it will be used from the executable's .config
  ///     - \\ cannot be used as the start of the configured path
  /// </summary>
  public class LocalFileManager : IFileManager
  {
    private IHostEnvironment _hostingEnvironment;
    private FileManagerSettings _settings;

    /// <summary>
    /// Initializes with the IOptions Mail Settings
    /// </summary>
    /// <param name="settings">IOptions MailSettings</param>
    public LocalFileManager(IHostEnvironment hostingEnvironment, IOptions<FileManagerSettings> settings)
    {
      _hostingEnvironment = hostingEnvironment;
      _settings = settings.Value;
    }

    /// <summary>
    /// Initializes the file manager with settings
    /// </summary>
    /// <param name="settings"></param>
    public LocalFileManager(FileManagerSettings settings)
    {
      _settings = settings;
    }

    public bool DirectoryExists(FilePathInfo Path)
    {
      if (Path == null) throw new ArgumentNullException(nameof(Path));
      return Directory.Exists(GetFullPath(Path));
    }

    public Task<bool> DirectoryExistsAsync(FilePathInfo Path)
    {
      return Task.Run(() => DirectoryExists(Path));
    }

    public void CreateDirectory(FilePathInfo Path)
    {
      if (Path == null) throw new ArgumentNullException(nameof(Path));
      Directory.CreateDirectory(GetFullPath(Path));
    }

    public bool FileExists(FilePathInfo Path)
    {
      if (Path == null) throw new ArgumentNullException(nameof(Path));
      return File.Exists(GetFullPathWithFilename(Path, false));
    }

    public Task<bool> FileExistsAsync(FilePathInfo Path)
    {
      return Task.Run(() => FileExists(Path));
    }

    /// <summary>
    /// Saves a file
    /// </summary>
    /// <param name="Stream">stream to save</param>
    /// <param name="Path">path of where to save</param>
    public void SaveFile(Stream Stream, FilePathInfo Path)
    {
      if (Stream == null) throw new ArgumentNullException(nameof(Stream));
      if (Path == null) throw new ArgumentNullException(nameof(Path));

      using (FileStream newFile = new FileStream(GetFullPathWithFilename(Path, true), FileMode.Create))
      {
        Stream.CopyTo(newFile);
        newFile.Close();
      }
    }

    /// <summary>
    /// saves a file async
    /// </summary>
    /// <param name="Stream">stream to save</param>
    /// <param name="Path">path of where to save</param>
    public async Task SaveFileAsync(Stream Stream, FilePathInfo Path)
    {
      if (Stream == null) throw new ArgumentNullException(nameof(Stream));
      if (Path == null) throw new ArgumentNullException(nameof(Path));

      using (FileStream newFile = new FileStream(GetFullPathWithFilename(Path, true), FileMode.Create))
      {
        await Stream.CopyToAsync(newFile);
        newFile.Close();
      }
    }

    /// <summary>
    /// Opens a file from the given path.  
    /// Optionally will create a file if one doesn't exist.
    /// </summary>
    /// <param name="Path">File path to open</param>
    /// <param name="CreateFile">Option to create file if it doesn't exist</param>
    /// <returns>File Stream for the specified path</returns>
    public Stream OpenFile(FilePathInfo Path, bool CreateFile = false)
    {
      if (Path == null) throw new ArgumentNullException(nameof(Path));

      string fullPath = GetFullPathWithFilename(Path, false);

      if (CreateFile && !File.Exists(fullPath))
      {
        if (!DirectoryExists(Path))
          CreateDirectory(Path);
        return File.Create(fullPath);
      }

      return File.OpenRead(fullPath);
    }

    public Task<Stream> OpenFileAsync(FilePathInfo Path, bool CreateFile = false)
    {
      return Task.Run(() => OpenFile(Path, CreateFile));
    }

    public bool DeleteFile(FilePathInfo Path)
    {
      if (Path == null) throw new ArgumentNullException(nameof(Path));

      try
      {
        File.Delete(GetFullPathWithFilename(Path, false));

        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }

    /// <summary>
    /// Deletes the file
    /// </summary>
    /// <param name="Path"></param>
    /// <returns></returns>
    public Task<bool> DeleteFileAsync(FilePathInfo Path)
    {
      return Task.Run(() => DeleteFile(Path));
    }

    /// <summary>
    /// Returns the absolute path for the specified path info using the root path.
    /// </summary>
    public string GetFullPathWithFilename(FilePathInfo Path, bool CreateDirectory)
    {
      string fullPath = GetFullPath(Path);

      if (String.IsNullOrEmpty(fullPath))
        return Path.FileName;
      else
      {
        if (CreateDirectory && !Directory.Exists(fullPath))
          Directory.CreateDirectory(fullPath);

        return System.IO.Path.Combine(fullPath, Path.FileName);
      }
    }

    #region Internal Methods

    private string GetFullPath(FilePathInfo Path)
    {
      string fullPath = String.Empty;

      if (String.IsNullOrEmpty(Path.Path))
        fullPath = GetRootPath();
      else
        fullPath = System.IO.Path.Combine(GetRootPath(), Path.Path);

      return fullPath;
    }

    string GetRootPath()
    {
      if (String.IsNullOrEmpty(_settings.RootPath))
      {
        if (_hostingEnvironment.IsDevelopment())
          return _hostingEnvironment.ContentRootPath;
        else
          return string.Empty;
      }
      else
      {
        string path = _settings.RootPath;

        if (path[0] == '\\' || path[0] == '/')
        {
          if (_hostingEnvironment.IsProduction() || _hostingEnvironment.IsStaging())
            throw new InvalidOperationException("If a root path begins with a '/' or '\', this must be used in a hosted environment.");

          path = Path.Combine(_hostingEnvironment.ContentRootPath, path.Trim('/', '\\'));
        }

        return path;
      }
    }

    #endregion
  }
}
