using SQLite;
using Windows.UI.Xaml;

namespace LocationAlarm.Data
{
    public class DataContext : IDataContext
    {
        private SQLiteAsyncConnection _asyncConnection;
        private string _databasePath = (string)Application.Current.Resources["DbConnectionString"];
        public SQLiteConnection Connection => new SQLiteConnection(_databasePath);

        public DataContext()
        {
        }
    }
}