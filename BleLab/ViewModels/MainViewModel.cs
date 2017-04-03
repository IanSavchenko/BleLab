using Caliburn.Micro;

namespace BleLab.ViewModels
{
    internal class MainViewModel : Conductor<object>
    {
        private readonly DeviceShellViewModel _deviceShellViewModel;
        private readonly AboutViewModel _aboutViewModel;
        private readonly SettingsViewModel _settingsViewModel;

        public MainViewModel(
            DeviceShellViewModel deviceShellViewModel, 
            AboutViewModel aboutViewModel, 
            SettingsViewModel settingsViewModel)
        {
            _deviceShellViewModel = deviceShellViewModel;
            _aboutViewModel = aboutViewModel;
            _settingsViewModel = settingsViewModel;
            ActivateItem(_deviceShellViewModel);
        }

        public void OpenDevice()
        {
            ActivateItem(_deviceShellViewModel);
        }

        public void OpenAbout()
        {
            ActivateItem(_aboutViewModel);
        }

        public void OpenSettings()
        {
            ActivateItem(_settingsViewModel);
        }
    }
}
