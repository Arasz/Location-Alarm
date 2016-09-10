using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.Persistence.FIleProvider
{
    internal class DefaultFileProvider : IFileProvider
    {
        /// <summary>
        /// Default data file extension 
        /// </summary>
        private const string DataFileExtension = @".ds";

        /// <summary>
        /// Default data file name 
        /// </summary>
        private const string DataFileName = @"data";

        /// <summary>
        /// Folder in which persistence file is stored 
        /// </summary>
        private readonly IStorageFolder _fileStorageFolder;

        /// <summary>
        /// Persistent file 
        /// </summary>
        private IStorageFile _storageFile;

        /// <summary>
        /// File name with extension 
        /// </summary>
        public string FullName => DataFileName + DataFileExtension;

        public DefaultFileProvider()
        {
            _fileStorageFolder = ApplicationData.Current.LocalFolder;
        }

        public DefaultFileProvider(IStorageFolder fileFileStorageFolder)
        {
            _fileStorageFolder = fileFileStorageFolder;
        }

        /// <summary>
        /// Gets persistence file, if file doesn't exist will be created 
        /// </summary>
        public async Task<IStorageFile> LoadPersistenceFileAsync()
        {
            try
            {
                await LoadFileAsync().ConfigureAwait(false);
            }
            catch (FileNotFoundException)
            {
                await CreateFileAsync().ConfigureAwait(false);
            }

            return _storageFile;
        }

        private async Task CreateFileAsync() => _storageFile = await _fileStorageFolder.CreateFileAsync(FullName);

        private async Task LoadFileAsync() => _storageFile = await _fileStorageFolder.GetFileAsync(FullName);
    }
}