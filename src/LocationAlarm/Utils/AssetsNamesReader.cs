using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace LocationAlarm.Utils
{
    public class AssetsNamesReader : IAssetsNamesReader
    {
        public string AssetsPath { get; } = Windows.ApplicationModel.Package.Current.InstalledLocation.Path +
                                            @"\Assets\";

        public async Task<IEnumerable<string>> ReadAsync(string assetsFolderName)
        {
            var storageFolder = await StorageFolder.GetFolderFromPathAsync(AssetsPath + assetsFolderName);
            var files = await storageFolder.GetFilesAsync();

            return files.Select(file => file.Name).ToList();
        }
    }
}