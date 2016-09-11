using AutoMapper;
using CoreLibrary.Data.DataModel.PersistentModel;
using CoreLibrary.Data.Persistence.Mapping;
using CoreLibrary.DataModel;
using CoreLibrary.DataModel.Persistence.Mapping;
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

        public AsyncTableQuery<GeolocationAlarm> CustomQuery { get; }

        public GelocationAlarmRepository(IRepository<Alarm> alarmsRepository)
        {
            InitializeMappings();
            _alarmsRepository = alarmsRepository;
        }

        public async Task DeleteAllAsync() => await _alarmsRepository.DeleteAllAsync().ConfigureAwait(false);

        public async Task<int> DeleteAsync(GeolocationAlarm entity) => await _alarmsRepository
            .DeleteAsync(Mapper.Map<GeolocationAlarm, Alarm>(entity))
            .ConfigureAwait(false);

        public void Dispose() => _alarmsRepository.Dispose();

        public async Task<IEnumerable<GeolocationAlarm>> FindAsync(Expression<Func<GeolocationAlarm, bool>> predicate)
        {
            var translatedPredicate = predicate.Compile();
            var result = await _alarmsRepository.FindAsync(alarm => translatedPredicate(Mapper.Map<Alarm, GeolocationAlarm>(alarm)))
                        .ConfigureAwait(false);
            return result.Select(Mapper.Map<Alarm, GeolocationAlarm>);
        }

        public async Task<IEnumerable<GeolocationAlarm>> GetAllAsync()
        {
            var result = await _alarmsRepository.GetAllAsync().ConfigureAwait(false);
            return result.Select(Mapper.Map<Alarm, GeolocationAlarm>);
        }

        public async Task<GeolocationAlarm> GetAsync(int id)
        {
            var result = await _alarmsRepository.GetAsync(id).ConfigureAwait(false);
            return Mapper.Map<Alarm, GeolocationAlarm>(result);
        }

        public async Task<int> InsertAllAsync(IEnumerable<GeolocationAlarm> entities)
        {
            var alarms = entities.Select(Mapper.Map<GeolocationAlarm, Alarm>);
            return await _alarmsRepository.InsertAllAsync(alarms).ConfigureAwait(false);
        }

        public async Task<int> InsertAsync(GeolocationAlarm entity)
        {
            var alarm = Mapper.Map<GeolocationAlarm, Alarm>(entity);
            return await _alarmsRepository.InsertAsync(alarm).ConfigureAwait(false);
        }

        public async Task<int> UpdateAllAsync(IEnumerable<GeolocationAlarm> entities)
        {
            var alarms = entities.Select(Mapper.Map<GeolocationAlarm, Alarm>);
            return await _alarmsRepository.UpdateAllAsync(alarms).ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(GeolocationAlarm entity)
        {
            var alarm = Mapper.Map<GeolocationAlarm, Alarm>(entity);
            return await _alarmsRepository.UpdateAsync(alarm).ConfigureAwait(false);
        }

        private static void InitializeMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<WeekDay, string>()
                    .ConvertUsing<WeekDayConverter>();

                cfg.CreateMap<string, WeekDay>()
                    .ConvertUsing<WeekDayConverter>();

                cfg.CreateMap<List<WeekDay>, string>()
                    .ConvertUsing<ListConverter>();

                cfg.CreateMap<string, List<WeekDay>>()
                    .ConvertUsing<ListConverter>();

                cfg.CreateMap<Alarm, GeolocationAlarm>()
                    .ForMember(alarm => alarm.Geoposition, options => options.ResolveUsing<BasicGeopositionResolver>());

                cfg.CreateMap<GeolocationAlarm, Alarm>();
            });
        }
    }
}