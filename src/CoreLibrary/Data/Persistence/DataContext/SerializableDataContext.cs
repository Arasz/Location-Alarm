using CoreLibrary.Data.Persistence.Common;
using CoreLibrary.DataModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.Persistence.DataContext
{
    /// <summary>
    /// Data context decorator which adds serialization 
    /// </summary>
    public class SerializableDataContext<TEntity> : IDataContext<TEntity>, ISelfSerializable
        where TEntity : class, IEntity
    {
        private const string _dataFileExtension = @".ds";

        private readonly IStorageFolder _storageFolder = ApplicationData.Current.LocalFolder;

        private IDataContext<TEntity> _dataContext;

        public IEnumerable<TEntity> Entities => _dataContext.Entities;

        private string FullDataFileName => nameof(TEntity) + _dataFileExtension;

        private IStorageFile StorageFile { get; set; }

        public SerializableDataContext(IDataContext<TEntity> dataContext)
        {
            _dataContext = dataContext;
        }

        public int Create(TEntity entity)
        {
            return _dataContext.Create(entity);
        }

        public void Delete(TEntity entity)
        {
            _dataContext.Delete(entity);
        }

        public void Delete(int id)
        {
            _dataContext.Delete(id);
        }

        public void DeleteAll()
        {
            _dataContext.DeleteAll();
        }

        public async Task DeserializeAsync()
        {
            if (await InitializeStorageFileAsync().ConfigureAwait(false)) return;

            using (var serializationStream = await StorageFile.OpenStreamForReadAsync().ConfigureAwait(false))
            {
                var serializer = new JsonSerializer();
                _dataContext = serializer.Deserialize<DataContext<TEntity>>(new JsonTextReader(new StreamReader(serializationStream)));
            }
        }

        public TEntity Read(int id)
        {
            return _dataContext.Read(id);
        }

        public IEnumerable<TEntity> ReadAll()
        {
            return _dataContext.ReadAll();
        }

        public async Task SerializeAsync()
        {
            await InitializeStorageFileAsync().ConfigureAwait(false);

            using (var serializationStream = await StorageFile.OpenStreamForWriteAsync().ConfigureAwait(false))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(new JsonTextWriter(new StreamWriter(serializationStream)), _dataContext);
            }
        }

        public void Update(TEntity entity)
        {
            _dataContext.Update(entity);
        }

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            _dataContext.UpdateAll(entities);
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