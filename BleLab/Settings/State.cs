using Windows.Storage;

namespace BleLab.Settings
{
    public class State : Keeper
    {
        public State() : base(
            new AppContainerKeeperStorage(
                ApplicationData.Current.LocalSettings
                .CreateContainer("State", ApplicationDataCreateDisposition.Always)))
        {
        }

        [KeeperProperty(DefaultValue = true)]
        public bool ConsoleOpen
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
    }
}
