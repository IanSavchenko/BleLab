using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands.Exceptions;
using BleLab.Model;

namespace BleLab.Commands.Characteristic
{
    public class WriteClientConfigDescriptorCommand : ClientConfigDescriptorCommandBase
    {
        public WriteClientConfigDescriptorCommand(CharacteristicInfo characteristicInfo, GattClientCharacteristicConfigurationDescriptorValue descriptor) : base(characteristicInfo)
        {
            Descriptor = descriptor;
        }

        protected override async Task DoExecuteAsync()
        {
            var result = await Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(Descriptor).AsTask().ConfigureAwait(false);
            if (result == GattCommunicationStatus.Unreachable)
                throw new DeviceUnreachableException();
        }
    }
}
