using CoreLibrary.Data.Persistence.Identity;
using Newtonsoft.Json;
using System;

namespace CoreLibrary.Data.Persistence.Json.Converters
{
    internal class IdentityGeneratorJsonConverter<TGenerator> : JsonConverter
        where TGenerator : class, IIdentityGenerator, new()
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(TGenerator) || true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => serializer.Deserialize<TGenerator>(reader);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);
    }
}