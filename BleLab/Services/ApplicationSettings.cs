using Windows.Storage;
using BleLab.Utils.Keeper;

namespace BleLab.Services
{
    public class ApplicationSettings : Keeper
    {
        public ApplicationSettings() : base(new AppContainerKeeperStorage(ApplicationData.Current.LocalSettings))
        {
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ConsoleShowTimestamps
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ConsoleShowTimestampsDiff
        {
            get => Get<bool>();
            set => Set(value);
        }
    }
}