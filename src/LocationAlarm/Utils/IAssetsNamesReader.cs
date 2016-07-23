using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationAlarm.Utils
{
    public interface IAssetsNamesReader
    {
        /// <summary>
        /// Reads file names from given folder. If empty string is given read all files from assets folder 
        /// </summary>
        /// <param name="assetsFolderName"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> ReadAsync(string assetsFolderName);
    }
}