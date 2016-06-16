using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LocationAlarm.Data
{
    public class LocationAlarmRepository
    {
        private readonly IDataContext _dataContext;

        public LocationAlarmRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public void DeleteAllLocationAlarms()
        {
            using (var connection = _dataContext.Connection)
            {
                connection.DeleteAll<Model.LocationAlarm>();
            }
        }

        public void DeleteLocationAlarm(Model.LocationAlarm alarm)
        {
            using (var connection = _dataContext.Connection)
            {
                connection.Delete<Model.LocationAlarm>(alarm);
            }
        }

        public void InsertLocationAlarm(Model.LocationAlarm alarm)
        {
            using (var connection = _dataContext.Connection)
            {
                connection.Update(alarm, typeof(Model.LocationAlarm));
            }
        }

        public IList<Model.LocationAlarm> ReadAllLocationAlarms()
        {
            using (var connection = _dataContext.Connection)
            {
                return connection.Table<Model.LocationAlarm>().ToList();
            }
        }

        public Model.LocationAlarm ReadLocationAlarm(Expression<Func<Model.LocationAlarm, bool>> predicate)
        {
            using (var connection = _dataContext.Connection)
            {
                var alarm = connection.Get(predicate);
                return alarm;
            }
        }

        public void UpdateLocationAlarm(Model.LocationAlarm alarm)
        {
            using (var connection = _dataContext.Connection)
            {
                connection.Insert(alarm, typeof(Model.LocationAlarm));
            }
        }

        private void CreateTable()
        {
            using (var connection = _dataContext.Connection)
            {
                connection.CreateTable<Model.LocationAlarm>();
            }
        }
    }
}