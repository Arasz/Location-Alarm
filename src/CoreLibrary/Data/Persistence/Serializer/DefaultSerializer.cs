using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.DataContext;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.Persistence.Serializer
{
    public class DefaultSerializer : ISerialize<IEntity>
    {
        private IStorageFile SerializationFile { get; }

        private JsonSerializer Serializer { get; }

        public DefaultSerializer(IStorageFile serializationFile, JsonSerializer serializer)
        {
            SerializationFile = serializationFile;
            Serializer = serializer;
        }

        public async Task<IDataContext<IEntity>> DeserializeAsync()
        {
            using (var fileReadStream = await SerializationFile.OpenStreamForReadAsync().ConfigureAwait(false))
            using (var reader = new StreamReader(fileReadStream))
            {
                var jsonReader = new JsonTextReader(reader);
                var deserializedContext = Serializer.Deserialize<IDataContext<IEntity>>(jsonReader);
                return deserializedContext;
            }
        }

        public async Task SerializeAsync(IDataContext<IEntity> dataContext)
        {
            using (var fileWriteStream = await SerializationFile.OpenStreamForWriteAsync().ConfigureAwait(false))
            using (var writer = new StreamWriter(fileWriteStream))
            {
                var jsonWriter = new JsonTextWriter(writer);
                Serializer.Serialize(jsonWriter, dataContext);
            }
        }
    }
}