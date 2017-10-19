using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Windows.Storage;

namespace BleLab.Settings
{
    public abstract class Keeper
    {
        protected readonly IKeeperStorage Storage;

        protected Keeper(IKeeperStorage storage)
        {
            Storage = storage;
        }

        protected bool Set<T>(T value, [CallerMemberName] string propertyName = null)
        {
            var key = KeyForProperty(propertyName);

            if (Storage.HasKey(key))
            {
                var currentValue = (T)Storage.GetValue(key);
                if (EqualityComparer<T>.Default.Equals(currentValue, value))
                    return false;
            }

            Storage.SetValue(key, value);
            return true;
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            var key = KeyForProperty(propertyName);

            if (Storage.HasKey(key))
                return (T)Storage.GetValue(key);

            var defaultValue = DefaultValueForProperty(key);
            if (defaultValue != null)
                return (T)defaultValue;

            return default(T);
        }


        protected CustomAttributeData AttributeForProperty(string propertyName)
        {
            var attribute = GetType().GetTypeInfo().GetDeclaredProperty(propertyName).CustomAttributes.FirstOrDefault(ca => ca.AttributeType == typeof(KeeperPropertyAttribute));
            return attribute;
        }

        protected string KeyForProperty(string propertyName)
        {
            var attribute = AttributeForProperty(propertyName);
            if (attribute == null)
                return propertyName;

            var persistentKey = (string)attribute.NamedArguments.First(t => t.MemberName == nameof(KeeperPropertyAttribute.PersistentKey)).TypedValue.Value;
            return persistentKey ?? propertyName;
        }

        protected object DefaultValueForProperty(string propertyName)
        {
            var attribute = AttributeForProperty(propertyName);

            var defaultValue = attribute?.NamedArguments.First(t => t.MemberName == nameof(KeeperPropertyAttribute.DefaultValue)).TypedValue.Value;
            return defaultValue;
        }
    }

    public interface IKeeperStorage
    {
        bool HasKey(string key);

        object GetValue(string key);

        void SetValue(string key, object value);
    }

    public class AppContainerKeeperStorage : IKeeperStorage
    {
        private readonly ApplicationDataContainer _container;

        public AppContainerKeeperStorage(ApplicationDataContainer container = null)
        {
            if (container == null)
                container = ApplicationData.Current.LocalSettings;

            _container = container;
        }

        public virtual bool HasKey(string key)
        {
            return _container.Values.ContainsKey(key);
        }

        public virtual object GetValue(string key)
        {
            return _container.Values[key];
        }

        public virtual void SetValue(string key, object value)
        {
            _container.Values[key] = value;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeeperPropertyAttribute : Attribute
    {
        public object DefaultValue { get; set; }

        public string PersistentKey { get; set; }
    }

    public class Settings : Keeper
    {
        public Settings() : base(new AppContainerKeeperStorage(ApplicationData.Current.LocalSettings))
        {
        }

        [KeeperProperty(DefaultValue = true, PersistentKey = "F6DF7E3F-308D-474B-8F6F-E10B7E309EB7")]
        public bool Test
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}
