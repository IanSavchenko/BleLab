using Windows.Storage;
using BleLab.Utils.Keeper;

namespace BleLab.Services
{
    public class ApplicationState : Keeper
    {
        public ApplicationState() : base(
            new AppContainerKeeperStorage(
                ApplicationData.Current.LocalSettings
                .CreateContainer("ApplicationState", ApplicationDataCreateDisposition.Always)))
        {
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ConsoleExpanded
        {
            get => Get<bool>();
            set => Set(value);
        }

        [KeeperProperty(DefaultValue = 10)]
        public int ConsoleFontSize
        {
            get => Get<int>();
            set => Set(value);
        }

        [KeeperProperty(DefaultValue = 200)]
        public int ConsoleHeight
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
