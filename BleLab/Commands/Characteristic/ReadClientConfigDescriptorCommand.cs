using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands.Exceptions;
using BleLab.Model;

namespace BleLab.Commands.Characteristic
{
    public class ReadClientConfigDescriptorCommand : ClientConfigDescriptorCommandBase
    {
        public ReadClientConfigDescriptorCommand(CharacteristicInfo gattCharacteristic) : base(gattCharacteristic)
        {
        }

        protected override async Task DoExecuteAsync()
        {
            var result = await Characteristic.ReadClientCharacteristicConfigurationDescriptorAsync().AsTask().ConfigureAwait(false);
            if (result.Status == GattCommunicationStatus.Unreachable)
            {
                throw new DeviceUnreachableException();
            }

            Descriptor = result.ClientCharacteristicConfigurationDescriptor;
        }
    }
}
