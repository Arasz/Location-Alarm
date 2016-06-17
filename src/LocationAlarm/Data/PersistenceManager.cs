using LocationAlarm.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Windows.Storage;

namespace LocationAlarm.Data
{
    public class PersistenceManager
    {
        private string _fileName = "data.xml";
        private IStorageFile _persistenceFile;
        private IStorageFolder _persistenceFolder = ApplicationData.Current.LocalFolder;

        public async Task<IEnumerable<AlarmModel>> LoadCollectionAsync()
        {
            if (!await ExistAsync(_fileName).ConfigureAwait(false))
                _persistenceFile = await _persistenceFolder.CreateFileAsync(_fileName);
            else
                _persistenceFile = await _persistenceFolder.GetFileAsync(_fileName);

            var basicProperties = await _persistenceFile.GetBasicPropertiesAsync();
            if (basicProperties.Size == 0) return new List<AlarmModel>();

            using (var stream = (await _persistenceFile.OpenAsync(FileAccessMode.Read)).AsStream())
            {
                var serializer = new XmlSerializer(typeof(AlarmModel[]));
                var deserialized = serializer.Deserialize(stream) as IEnumerable<AlarmModel>;
                return deserialized ?? new List<AlarmModel>();
            }
        }

        public async Task SaveCollectionAsync(IEnumerable<AlarmModel> locationAlarms)
        {
            if (!await ExistAsync(_fileName).ConfigureAwait(false))
                _persistenceFile = await _persistenceFolder.CreateFileAsync(_fileName);
            else
                _persistenceFile = await _persistenceFolder.GetFileAsync(_fileName);

            using (var stream = (await _persistenceFile.OpenAsync(FileAccessMode.ReadWrite)).AsStream())
            {
                var serializer = new XmlSerializer(typeof(AlarmModel[]));
                serializer.Serialize(stream, locationAlarms.ToArray());
            }
        }

        private async Task<bool> ExistAsync(string fileName)
            => (await _persistenceFolder.GetFilesAsync())
            .Any(file => file.Name == fileName);
    }
}