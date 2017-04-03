using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BleLab.Views
{
    public sealed partial class DeviceInfoView : UserControl
    {
        public DeviceInfoView()
        {
            this.InitializeComponent();
        }

        private void More_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as Button);
        }
    }
}
