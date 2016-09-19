using System.Collections.Generic;

namespace LocationAlarm.Navigation.Bundle
{
    public class Bundle
    {
        private IDictionary<string, object> _dataDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Returns element which key equal to given type 
        /// </summary>
        public T Get<T>()
        {
            var value = Get(typeof(T).Name);
            return value is T ? (T)value : default(T);
        }

        public T Get<T>(string key)
        {
            var value = Get(key);
            return value is T ? (T)value : default(T);
        }

        public object Get(string key) => _dataDictionary.ContainsKey(key) ? _dataDictionary[key] : null;

        public void Insert<T>(string key, T value) => Insert(key, value);

        public void Insert(string key, object value) => _dataDictionary[key] = value;
    }
}