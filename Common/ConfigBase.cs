using System;
using System.Collections.Generic;
using System.Reflection;
using JsonFx.Json;

namespace HPG.Common
{
    public abstract class ConfigBase<T> where T : class
    {
        #region Singltone

        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Config doesnt created. Use Constructor to create it");
                return _instance;
            }
        }

        protected ConfigBase()
        {
            if (_instance == null)
                _instance = this as T;
        }
        #endregion

        public void AppendConfig(string configJson)
        {
            var newConfig = JsonReader.Deserialize(configJson) as Dictionary<string, object>;
            AppendConfig(newConfig);
        }

        public void AppendConfig(Dictionary<string, object> config)
        {
            var newConfig = config;
            if (newConfig == null)
                return;
            PropertyInfo[] props = GetType().GetProperties();
            Dictionary<string, PropertyInfo> namedProps = new Dictionary<string, PropertyInfo>();

            foreach (PropertyInfo prop in props)
            {
                string name = prop.Name;
                string propertyName = JsonNameAttribute.GetJsonName(prop);
                if (!String.IsNullOrEmpty(propertyName))
                {
                    name = propertyName;
                }
                namedProps.Add(name, prop);
            }

            foreach (var section in newConfig)
            {
                if (namedProps.ContainsKey(section.Key))
                {
                    namedProps[section.Key].SetValue(this, JsonReader.CoerceType(namedProps[section.Key].PropertyType, section.Value), null);
                }
            }
            ApplyData();
        }

        private object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        protected abstract void ApplyData();
    }

}
