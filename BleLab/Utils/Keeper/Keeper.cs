using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BleLab.Utils.Keeper
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

            var persistentKeyProperty = attribute.NamedArguments.FirstOrDefault(t => t.MemberName == nameof(KeeperPropertyAttribute.PersistentKey));
            if (persistentKeyProperty.Equals(default(CustomAttributeNamedArgument)))
            {
                return propertyName;
            }

            var persistentKey = (string)persistentKeyProperty.TypedValue.Value;
            return persistentKey ?? propertyName;
        }

        protected object DefaultValueForProperty(string propertyName)
        {
            var attribute = AttributeForProperty(propertyName);
            if (attribute == null)
                return null;

            var defaultValueProperty = attribute.NamedArguments.FirstOrDefault(t => t.MemberName == nameof(KeeperPropertyAttribute.DefaultValue));
            if (defaultValueProperty.Equals(default(CustomAttributeNamedArgument)))
                return null;

            var defaultValue = defaultValueProperty.TypedValue.Value;
            return defaultValue;
        }
    }
}
