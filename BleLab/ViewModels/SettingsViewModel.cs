using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class SettingsViewModel : PropertyChangedBase
    {
        private readonly ApplicationSettings _applicationSettings;

        public SettingsViewModel(ApplicationSettings applicationSettings)
        {
            _applicationSettings = applicationSettings;
        }

        public bool ConsoleShowTime
        {
            get => _applicationSettings.ConsoleShowTimestamps;
            set => _applicationSettings.ConsoleShowTimestamps = value;
        }

        public bool ConsoleShowTimeDiff
        {
            get => _applicationSettings.ConsoleShowTimestampsDiff;
            set => _applicationSettings.ConsoleShowTimestampsDiff = value;
        }
    }
}