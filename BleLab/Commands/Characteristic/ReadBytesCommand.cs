using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
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
            var result = await Characteristic.ReadValueAsync(CacheMode).AsTask().ConfigureAwait(false);
            if (result.Status == GattCommunicationStatus.Unreachable)
            {
                Status = CommandStatus.Unreachable;
                return;
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
