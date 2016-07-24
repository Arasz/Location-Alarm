using CoreLibrary.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CoreLibrary.DataModel
{
    public class GelocationAlarmRepository
    {
        private readonly ISessionFactory _sessionFactory;

        public GelocationAlarmRepository(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public int Create(GeolocationAlarm alarm)
        {
            using (var session = _sessionFactory.OpenSessionAsync())
            {
            }
        }

        public void Delete(GeolocationAlarm alarm)
        {
        }

        public void DeleteAll()
        {
        }

        public GeolocationAlarm Read(int index)
        {
            return null;
        }

        public IEnumerable<GeolocationAlarm> ReadAll()
        {
            return null;
        }

        public GeolocationAlarm ReadMatching(Expression<Func<GeolocationAlarm, bool>> criteria)
        {
            return null;
        }

        public bool Update(GeolocationAlarm alarm)
        {
            return false;
        }
    }
}