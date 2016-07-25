using CoreLibrary.Data.Persistence;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreLibrary.DataModel
{
    public class GelocationAlarmRepository
    {
        private readonly ISessionFactory<GeolocationAlarm> _sessionFactory;

        public GelocationAlarmRepository(ISessionFactory<GeolocationAlarm> sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public async Task<int> CreateAsync(GeolocationAlarm alarm)
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
            {
                var id = session.Create(alarm);
                alarm.Id = id;
                return id;
            }
        }

        public async Task DeleteAllAsync()
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
                session.DeleteAll();
        }

        public async Task DeleteAsync(GeolocationAlarm alarm)
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
                session.Delete(alarm);
        }

        public async Task<IEnumerable<GeolocationAlarm>> ReadAllAsync()
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
                return session.ReadAll();
        }

        public async Task<GeolocationAlarm> ReadAsync(int index)
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
                return session.Read(index);
        }

        public async Task<GeolocationAlarm> ReadMatchingAsync(Expression<Func<GeolocationAlarm, bool>> criteria)
        {
            throw new NotImplementedException("Will be implemented in future release");
        }

        public async Task Update(GeolocationAlarm alarm)
        {
            using (var session = await _sessionFactory.OpenSessionAsync().ConfigureAwait(false))
                session.Update(alarm);
        }
    }
}