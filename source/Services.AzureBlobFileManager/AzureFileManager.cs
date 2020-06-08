using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Services.FileManager;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services.AzureBlobFileManager
{
    public class AzureBlobFileManager : IFileManager
    {
        const long TWO_HUNDRED_MEG_IN_BYTES = 104857600;

        public AzureBlobFileManager(IOptions<AzureManagerSettings> azureManagerSettings)
        {
            if (string.IsNullOrEmpty(azureManagerSettings.Value.AzureStorageConnectionString))
            {
                throw new InvalidOperationException("Connection String Setting not setup.  AzureBlobService's constructor must be provided the AzureBlobConnectionString from your web.config.");
            }

            ConnectionStringSettingName = azureManagerSettings.Value.AzureStorageConnectionString;
            _internalCloudAccessAccount = CloudStorageAccount.Parse(azureManagerSettings.Value.AzureStorageConnectionString);
            _internalBlobClient = _internalCloudAccessAccount.CreateCloudBlobClient();
            CreateBlobContainersIfNotFound = false;
            OverwriteExistingFiles = true;
            _RootBLOBContainerName = azureManagerSettings.Value.AzureDefaultContainerName;
        }

        public AzureBlobFileManager(AzureManagerSettings azureManagerSettings)
        {
            if (string.IsNullOrEmpty(azureManagerSettings.AzureStorageConnectionString))
            {
                throw new InvalidOperationException("Connection String Setting not setup.  AzureBlobService's constructor must be provided the AzureBlobConnectionString from your web.config.");
            }

            ConnectionStringSettingName = azureManagerSettings.AzureStorageConnectionString;
            _internalCloudAccessAccount = CloudStorageAccount.Parse(azureManagerSettings.AzureStorageConnectionString);
            _internalBlobClient = _internalCloudAccessAccount.CreateCloudBlobClient();
            CreateBlobContainersIfNotFound = false;
            OverwriteExistingFiles = true;
            _RootBLOBContainerName = azureManagerSettings.AzureDefaultContainerName;
        }

        public bool CreateBlobContainersIfNotFound { get; set; }
        public bool OverwriteExistingFiles { get; set; }

        private string ConnectionStringSettingName { get; set; }
        private const string FilenameMissingError = "The filename cannot be null or empty.";
        private string _RootBLOBContainerName = "";
        private CloudStorageAccount _internalCloudAccessAccount;
        private CloudBlobClient _internalBlobClient;

        public void CreateDirectory(FilePathInfo Path)
        {
            //Directories are created by the file, I don't think there is a need for it right now
            CreateDirectoryHelper(Path);
        }

        public bool DirectoryExists(FilePathInfo Path)
        {
            return DirectoryExistsHelper(Path);
        }

        public Task<bool> DirectoryExistsAsync(FilePathInfo Path)
        {
            return Task.Run(() => DirectoryExistsHelper(Path));
        }

        public bool FileExists(FilePathInfo Path)
        {
            return FileExistsHelper(Path).GetAwaiter().GetResult();
        }

        public Task<bool> FileExistsAsync(FilePathInfo Path)
        {
            return FileExistsHelper(Path);
        }

        public Stream OpenFile(FilePathInfo Path, bool CreateFile)
        {
            return OpenFileHelper(Path, CreateFile).GetAwaiter().GetResult();
        }

        public Task<Stream> OpenFileAsync(FilePathInfo Path, bool CreateFile = false)
        {
            return OpenFileHelper(Path, CreateFile);
        }

        public void SaveFile(Stream Stream, FilePathInfo Path)
        {
            SaveFileHelper(Stream, Path).GetAwaiter().GetResult();
        }

        public Task SaveFileAsync(Stream Stream, FilePathInfo Path)
        {
            return SaveFileHelper(Stream, Path);
        }

        public bool DeleteFile(FilePathInfo Path)
        {
            return DeleteFileAsync(Path).GetAwaiter().GetResult();
        }

        public Task<bool> DeleteFileAsync(FilePathInfo Path)
        {
            if (string.IsNullOrEmpty(Path.FileName))
            {
                throw new ArgumentException(FilenameMissingError);
            }

            CloudBlobContainer myContainer = _internalBlobClient.GetContainerReference(_RootBLOBContainerName);

            return myContainer.GetBlockBlobReference(GetFullPath(Path.FileName, Path.Path, false)).DeleteIfExistsAsync();
        }

        protected void CreateDirectoryHelper(FilePathInfo Path)
        {
            throw new NotImplementedException();
        }

        protected bool DirectoryExistsHelper(FilePathInfo Path)
        {
            throw new NotImplementedException();
        }

        private Task<bool> FileExistsHelper(FilePathInfo Path)
        {
            CloudBlobContainer myContainer = _internalBlobClient.GetContainerReference(_RootBLOBContainerName);
            string fullPath = GetFullPath(Path.FileName, Path.Path, false);
            return myContainer.GetBlockBlobReference(fullPath).ExistsAsync();
        }

        private async Task<Stream> OpenFileHelper(FilePathInfo path, bool createFile)
        {
            if (string.IsNullOrEmpty(path.FileName))
            {
                throw new ArgumentException(FilenameMissingError);
            }

            CloudBlobContainer myContainer = _internalBlobClient.GetContainerReference(_RootBLOBContainerName);

            string fullPath = GetFullPath(path.FileName, path.Path, false);

            CloudBlockBlob ExistingBlobToAccess = myContainer.GetBlockBlobReference(fullPath);
            var cbbExists = await ExistingBlobToAccess.ExistsAsync();
            if (cbbExists)
            {
                if (ExistingBlobToAccess.Properties.Length > TWO_HUNDRED_MEG_IN_BYTES)
                {
                    var customStream = new AzureCustomStream(ExistingBlobToAccess);

                    return customStream;
                }
                else
                {
                    MemoryStream BlobMemoryStream = new MemoryStream();

                    await ExistingBlobToAccess.DownloadToStreamAsync(BlobMemoryStream);
                    BlobMemoryStream.Position = 0;
                    return BlobMemoryStream;
                }
            }
            else if (createFile)
            {
                //Create an empty file and pass back the stream
                using (var ms = new MemoryStream())
                {
                    await SaveFileHelper(ms, path);
                }

                return await OpenFileHelper(path, false);
            }
            else
            {
                throw new FileNotFoundException("Requested File not Found in this Container");
            }
        }

        private async Task SaveFileHelper(Stream Stream, FilePathInfo Path)
        {
            if (Stream == null)
                throw new ArgumentNullException(nameof(Stream));

            if (string.IsNullOrEmpty(Path.FileName))
                throw new ArgumentNullException(nameof(Path.FileName));

            CloudBlobContainer myContainer = _internalBlobClient.GetContainerReference(_RootBLOBContainerName);

            if (!await myContainer.ExistsAsync())
            {
                if (CreateBlobContainersIfNotFound)
                    await myContainer.CreateIfNotExistsAsync();
                else
                    throw new ArgumentException("Blob Container does not exist and auto-creation is not allowed");
            }

            string pathString = Path.Path;

            if (pathString != null && !pathString.EndsWith("/"))
                pathString = string.Concat(pathString, "/");

            CloudBlockBlob NewBlobToCreate = myContainer.GetBlockBlobReference(pathString + Path.FileName);
            if (!string.IsNullOrWhiteSpace(Path.MimeType))  //Set the content Type if needed
                NewBlobToCreate.Properties.ContentType = Path.MimeType;

            await NewBlobToCreate.UploadFromStreamAsync(Stream);
        }

        private string GetFullPath(string filePath)
        {
            return GetFullPath(filePath, null, false);
        }

        private string GetFullPath(string filename, string path, bool create)
        {
            string fullPath = string.Empty;

            if (string.IsNullOrEmpty(path))
            {
                return filename;
            }
            else
            {
                return Path.Combine(path, filename).Replace("\\", "/");
            }
        }
    }

    public class AzureCustomStream : Stream
    {
        //The buffer could be in a helper class
        const long MEG_IN_BYTES = 1048576;
        protected CloudBlockBlob _blob;

        public AzureCustomStream(CloudBlockBlob blob)
        {
            _blob = blob;
            _position = 0;
            _length = blob.Properties.Length;
            _bufferPosition = 0;

            FetchNextBuffer();
        }

        protected byte[] _buffer;
        protected long _bufferLength;
        protected long _bufferPosition;
        protected long _bufferStartPosition;
        protected long _position;
        protected long _length;

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => _length;

        public override long Position { get => _position; set => Seek(value, SeekOrigin.Begin); }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        protected void FetchNextBuffer()
        {
            long countToPull = MEG_IN_BYTES;
            if (_position + countToPull > _length)
            {
                countToPull = _length - _position;
            }

            _buffer = new byte[countToPull];
            if (countToPull == 0)
            {
                return;
            }

            var bdrb = _blob.DownloadRangeToByteArrayAsync(_buffer, 0, _position, countToPull).GetAwaiter().GetResult();

            _bufferLength = bdrb;
            _bufferPosition = 0;
            _bufferStartPosition = _position;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int amountRead = ReadHelper(buffer, offset, count);
            if (amountRead == 0)
            {
                return 0;
            }
            bool endOfFile = false;
            int totalAmountRead = amountRead;
            while (totalAmountRead < count && !endOfFile)
            {
                int newAmountRead = ReadHelper(buffer, offset + totalAmountRead, count - totalAmountRead);
                totalAmountRead += newAmountRead;
                if (newAmountRead == 0)
                {
                    endOfFile = true;
                }
            }

            return totalAmountRead;
        }

        protected int ReadHelper(byte[] buffer, int offset, int count)
        {
            long countToPull = count;
            //Can't read paste the end of the file
            if (_position + count > _length)
            {
                countToPull = _length - _position;
            }
            //No more data to read
            if (countToPull == 0)
            {
                return 0;
            }

            //Can't read past end of buffer, this must be less than count still
            if (countToPull > _bufferLength - _bufferPosition)
            {
                countToPull = _bufferLength - _bufferPosition;
            }

            for (int i = 0; i < countToPull; i++)
            {
                buffer[i + offset] = _buffer[i + _bufferPosition];
            }

            _position += countToPull;
            _bufferPosition += countToPull;

            if (_bufferPosition >= _bufferLength)
            {
                FetchNextBuffer();
            }

            return (int)countToPull;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            var oldPosition = _position;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;
                case SeekOrigin.Current:
                    _position += offset;
                    break;
                case SeekOrigin.End:
                    _position = _length - offset;
                    break;
            }

            if (_position >= _length || _position < 0)
            {
                _position = oldPosition;
                throw new Exception("offset not valid!");
            }

            if (_position != oldPosition)
            {
                FetchNextBuffer();
            }

            return _position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
