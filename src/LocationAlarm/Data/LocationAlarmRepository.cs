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
            CreateTable();
        }

        public void DeleteAllLocationAlarms()
        {
            using (var connection = _dataContext.Connection)
            {
                connection.RunInTransaction(() =>
                {
                    connection.DeleteAll<Model.LocationAlarm>();
                });
            }
        }

        public void DeleteLocationAlarm(Model.LocationAlarm alarm)
        {
            using (var connection = _dataContext.Connection)
            {
                connection.RunInTransaction(() =>
                {
                    connection.Delete(alarm);
                });
            }
        }

        public int InsertLocationAlarm(Model.LocationAlarm alarm)
        {
            using (var connection = _dataContext.Connection)
            {
                int primaryKey = -1;
                connection.RunInTransaction(() =>
                {
                    primaryKey = connection.Insert(alarm, typeof(Model.LocationAlarm));
                });
                return primaryKey;
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
                connection.RunInTransaction(() =>
                {
                    connection.Update(alarm, typeof(Model.LocationAlarm));
                });
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