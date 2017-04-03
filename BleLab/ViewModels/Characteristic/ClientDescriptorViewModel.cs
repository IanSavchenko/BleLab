using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class ClientDescriptorViewModel : PropertyChangedBase
    {
        private string _info;
        private bool _enabled;

        public ClientDescriptorViewModel(GattClientCharacteristicConfigurationDescriptorValue descriptor, string title)
        {
            Descriptor = descriptor;
            Title = title;
            Info = "State unknown";
        }

        public GattClientCharacteristicConfigurationDescriptorValue Descriptor { get; private set; }

        public string Title { get; private set; }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == _enabled) return;
                _enabled = value;

                NotifyOfPropertyChange();
                Info = null;
            }
        }

        public string Info
        {
            get { return _info; }
            set
            {
                if (value == _info) return;
                _info = value;
                NotifyOfPropertyChange();
            }
        }

        public void SetStateWritten()
        {
            Info = "State written";
        }

        public void SetStateRead(bool enabled)
        {
            Enabled = enabled;
            Info = "State updated";
        }
    }
}
