using CoreLibrary.Data.DataModel.Session;
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

        public void Delete(GeolocationAlarm alarm)
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                session.Delete(alarm);
            }
        }

        public void DeleteAll()
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                session.DeleteAll<GeolocationAlarm>();
            }
        }

        public int Insert(GeolocationAlarm alarm)
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                return session.Create(alarm); ;
            }
        }

        public GeolocationAlarm Read(int index)
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                return session.Read<GeolocationAlarm>(index);
            }
        }

        public IEnumerable<GeolocationAlarm> ReadAll()
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                return session.ReadAll<GeolocationAlarm>();
            }
        }

        public IEnumerable<GeolocationAlarm> ReadMatching(Expression<Func<GeolocationAlarm, bool>> criteria)
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                return session.Read(criteria);
            }
        }

        public void Update(GeolocationAlarm alarm)
        {
            using (var session = _sessionFactory.CurrentSession)
            {
                session.Update(alarm); ;
            }
        }
    }
}