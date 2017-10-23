using System;

namespace BleLab.Utils.Keeper
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KeeperPropertyAttribute : Attribute
    {
        public object DefaultValue { get; set; }

        public string PersistentKey { get; set; }
    }
}