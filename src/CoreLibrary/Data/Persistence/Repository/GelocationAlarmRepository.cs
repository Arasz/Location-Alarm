using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Mapping;
using CoreLibrary.DataModel;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CoreLibrary.Data.Persistence.Repository
{
    public class GelocationAlarmRepository : IRepository<GeolocationAlarm>
    {
        private readonly IRepository<Alarm> _alarmsRepository;
        private IMapper<Alarm, GeolocationAlarm> fromPersistenceMapper;
        private IMapper<GeolocationAlarm, Alarm> toPersistenceMapper;
        public AsyncTableQuery<GeolocationAlarm> CustomQuery { get; }

        public GelocationAlarmRepository(IRepository<Alarm> alarmsRepository)
        {
            InitializeMappings();
            _alarmsRepository = alarmsRepository;
        }

        public async Task DeleteAllAsync() => await _alarmsRepository.DeleteAllAsync().ConfigureAwait(false);

        public async Task<int> DeleteAsync(GeolocationAlarm entity) => await _alarmsRepository
            .DeleteAsync(toPersistenceMapper.Map(entity))
            .ConfigureAwait(false);

        public void Dispose() => _alarmsRepository.Dispose();

        public async Task<IEnumerable<GeolocationAlarm>> FindAsync(Expression<Func<GeolocationAlarm, bool>> predicate)
        {
            var translatedPredicate = predicate.Compile();
            var result = await _alarmsRepository.FindAsync(alarm => translatedPredicate(fromPersistenceMapper.Map(alarm, null)))
                        .ConfigureAwait(false);
            return result.Select(alarm => fromPersistenceMapper.Map(alarm));
        }

        public async Task<IEnumerable<GeolocationAlarm>> GetAllAsync()
        {
            var result = await _alarmsRepository.GetAllAsync().ConfigureAwait(false);
            return result.Select(alarm => fromPersistenceMapper.Map(alarm));
        }

        public async Task<GeolocationAlarm> GetAsync(int id)
        {
            var result = await _alarmsRepository.GetAsync(id).ConfigureAwait(false);
            return fromPersistenceMapper.Map(result);
        }

        public async Task<int> InsertAllAsync(IEnumerable<GeolocationAlarm> entities)
        {
            var alarms = entities.Select(alarm => toPersistenceMapper.Map(alarm));
            return await _alarmsRepository.InsertAllAsync(alarms).ConfigureAwait(false);
        }

        public async Task<int> InsertAsync(GeolocationAlarm entity)
        {
            var alarm = toPersistenceMapper.Map(entity);
            return await _alarmsRepository.InsertAsync(alarm).ConfigureAwait(false);
        }

        public async Task<int> UpdateAllAsync(IEnumerable<GeolocationAlarm> entities)
        {
            var alarms = entities.Select(alarm => toPersistenceMapper.Map(alarm));
            return await _alarmsRepository.UpdateAllAsync(alarms).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(GeolocationAlarm entity)
        {
            var alarm = toPersistenceMapper.Map(entity);
            return await _alarmsRepository.UpdateAsync(alarm).ConfigureAwait(false);
        }

        private void InitializeMappings()
        {
            toPersistenceMapper = new Mapper<GeolocationAlarm, Alarm>(new Configuration<GeolocationAlarm, Alarm>()
                .AddMapping(new AlarmMapper()));

            fromPersistenceMapper = new Mapper<Alarm, GeolocationAlarm>(new Configuration<Alarm, GeolocationAlarm>()
                .AddMapping(new AlarmMapper()));
        }
    }
}