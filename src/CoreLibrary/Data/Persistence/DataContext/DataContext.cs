using CoreLibrary.Data.DataModel.Base;
using CoreLibrary.Data.Persistence.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreLibrary.Data.Persistence.DataContext
{
    /// <summary>
    /// Encapsulates persistent data 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class DataContext<TEntity> : IDataContext<TEntity>
        where TEntity : class, IEntity
    {
        [DataMember, JsonProperty]
        private Dictionary<int, TEntity> _entities = new Dictionary<int, TEntity>();

        public IEnumerable<TEntity> Entities => _entities.Values;

        [DataMember, JsonProperty, JsonConverter(typeof(IdentityGeneratorConverter<IdentityGenerator>))]
        public IIdentityGenerator IdentityGenerator { get; set; }

        public DataContext(IIdentityGenerator identityGenerator)
        {
            IdentityGenerator = identityGenerator;
        }

        public int Create(TEntity entity)
        {
            IdentityGenerator.AssignUniqueIdentity(entity);
            _entities[entity.Id] = entity;
            return entity.Id;
        }

        public void Delete(TEntity entity)
        {
            IdentityGenerator.RecycleIdentity(entity);
            _entities[entity.Id] = null;
        }

        public void Delete(int id) => _entities[id] = null;

        public void DeleteAll() => _entities.Clear();

        public TEntity Read(int id) => _entities[id];

        public IEnumerable<TEntity> ReadAll() => _entities.Values;

        public void Update(TEntity entity) => _entities[entity.Id] = entity;

        public void UpdateAll(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                _entities[entity.Id] = entity;
        }
    }

    internal class IdentityGeneratorConverter<TGenrator> : JsonConverter
        where TGenrator : class, IIdentityGenerator, new()
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TGenrator);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => serializer.Deserialize<TGenrator>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
}