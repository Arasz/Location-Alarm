using SQLite;

namespace LocationAlarm.Data
{
    public interface IDataContext
    {
        SQLiteConnection Connection { get; }
    }
}