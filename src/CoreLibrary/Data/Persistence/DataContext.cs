using CoreLibrary.Data.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data
{
    /// <summary>
    /// Encapsulates persistent data 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [DataContract]
    public class DataContext<TEntity> : IDataContext<TEntity>
    {
        private const string _dataFileExtension = @".ds";

        private readonly IStorageFolder _storageFolder = ApplicationData.Current.LocalFolder;

        [DataMember]
        private List<TEntity> _entities = new List<TEntity>();

        public IEnumerable<TEntity> Entities => _entities;

        private string FullDataFileName => nameof(TEntity) + _dataFileExtension;

        private IStorageFile StorageFile { get; set; }

        public async Task DeserializeAsync()
        {
            if (await InitializeStorageFileAsync().ConfigureAwait(false)) return;

            using (var serializationStream = await StorageFile.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                var serializer = new JsonSerializer();
                _entities = serializer.Deserialize<List<TEntity>>(new JsonTextReader(new StreamReader(serializationStream)));
            }
        }

        public async Task SerializeAsync()
        {
            await InitializeStorageFileAsync().ConfigureAwait(false);

            using (var serializationStream = await StorageFile.OpenStreamForWriteAsync().ConfigureAwait(false))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(new JsonTextWriter(new StreamWriter(serializationStream)), _entities);
            }
        }

        /// <summary>
        /// Gets handle to storage file, if don't exist creates it 
        /// </summary>
        /// <returns> True if file was created </returns>
        private async Task<bool> InitializeStorageFileAsync()
        {
            try
            {
                StorageFile = await _storageFolder.GetFileAsync(FullDataFileName);
                return false;
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                StorageFile = await _storageFolder.CreateFileAsync(FullDataFileName);
                return true;
            }
        }
    }
}