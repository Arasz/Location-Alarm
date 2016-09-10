using System.Threading.Tasks;
using Windows.Storage;

namespace CoreLibrary.Data.Persistence.FIleProvider
{
    /// <summary>
    /// Loads file used for data persistence 
    /// </summary>
    internal interface IFileProvider
    {
        /// <summary>
        /// Loads file used for serialization 
        /// </summary>
        Task<IStorageFile> LoadPersistenceFileAsync();
    }
}