using System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BleLab.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private readonly UISettings _uiSettings;

        public MainView()
        {
            this.InitializeComponent();
            OpenDevice.IsChecked = true;

            _uiSettings = new UISettings();
            _uiSettings.ColorValuesChanged += UiSettingsOnColorValuesChanged;

            SwitchToTheme(ElementTheme.Default);
        }

        public void SwitchToTheme(ElementTheme theme)
        {
            Execute.OnUIThreadAsync(() =>
            {
                this.RequestedTheme = theme;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = (Color)Resources["SystemAltHighColor"];
                    titleBar.ButtonForegroundColor = (Color)Resources["SystemBaseHighColor"];
                    titleBar.BackgroundColor = (Color)Resources["SystemAltHighColor"];
                    titleBar.ForegroundColor = (Color)Resources["SystemBaseHighColor"];
                }
            });
        }

        private void HamburgerButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationPane.IsPaneOpen = !NavigationPane.IsPaneOpen;
        }

        private void MenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationPane.IsPaneOpen = false;
        }

        private void UiSettingsOnColorValuesChanged(UISettings sender, object args)
        {
            SwitchToTheme(ElementTheme.Default);
        }
    }
}
