using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BleLab.Commands.Exceptions;
using BleLab.Model;

namespace BleLab.Commands.Characteristic
{
    public class ReadBytesCommand : BytesCommandBase
    {
        public ReadBytesCommand(CharacteristicInfo characteristicInfo, BluetoothCacheMode cacheMode) : base(characteristicInfo)
        {
            CacheMode = cacheMode;
        }

        public BluetoothCacheMode CacheMode { get; }

        protected override async Task DoExecuteAsync()
        {
            var result = await WindowsRuntimeSystemExtensions.AsTask(Characteristic.ReadValueAsync(CacheMode)).ConfigureAwait(false);
            if (result.Status == GattCommunicationStatus.Unreachable)
            {
                throw new DeviceUnreachableException();
            }

            Bytes = ToArraySafe(result.Value);
        }

        private static byte[] ToArraySafe(IBuffer buffer)
        {
            if (buffer?.Length > 0)
                return buffer.ToArray();

            return null;
        }
    }
}
