using Windows.UI.Xaml.Controls;
using BleLab.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BleLab.Views
{
    public sealed partial class DeviceView : UserControl
    {
        public DeviceView()
        {
            this.InitializeComponent();
        }

        public DeviceViewModel ViewModel => (DeviceViewModel)DataContext;
    }
}
