using Windows.Storage;

namespace BleLab.Utils.Keeper
{
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
}