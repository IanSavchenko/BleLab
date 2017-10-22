using Windows.Storage;

namespace BleLab.Settings
{
    public class Settings : Keeper
    {
        public Settings() : base(new AppContainerKeeperStorage(ApplicationData.Current.LocalSettings))
        {
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ShowTimestampsInConsole
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ShowTimestampsDiffInConsole
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}