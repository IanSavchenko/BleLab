using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
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
                Status = CommandStatus.Unreachable;
                return;
            }

            Descriptor = result.ClientCharacteristicConfigurationDescriptor;
        }
    }
}
