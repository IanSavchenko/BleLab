using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Model;

namespace BleLab.Commands.Characteristic
{
    public abstract class ClientConfigDescriptorCommandBase : CharacteristicCommandBase
    {
        public ClientConfigDescriptorCommandBase(CharacteristicInfo gattCharacteristic) : base(gattCharacteristic)
        {
        }

        public GattClientCharacteristicConfigurationDescriptorValue Descriptor { get; protected set; }
    }
}
