using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using BleLab.Model;
using BleLab.Utils;

namespace BleLab.Services
{
    public class DeviceController
    {
        private readonly InfoManager _infoManager;

        public DeviceController(InfoManager infoManager)
        {
            _infoManager = infoManager;
        }

        public event EventHandler DeviceConnected;

        public event EventHandler DeviceDisconnecting;

        public event EventHandler DeviceDisconnected;

        public BluetoothLEDevice ConnectedDevice { get; private set; }

        public async Task Connect(DeviceInfo deviceInfo)
        {
            Disconnect();
            ConnectedDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.DeviceId);

            if (ConnectedDevice == null)
                return;

            deviceInfo.MacAddress = ConnectedDevice.BluetoothAddress.ToMacAddressString();
            deviceInfo.Name = ConnectedDevice.Name;

            _infoManager.SaveDevice(deviceInfo);

            DeviceConnected?.Invoke(this, EventArgs.Empty);
        }

        public void Disconnect()
        {
            if (ConnectedDevice == null)
                return;
            
            DeviceDisconnecting?.Invoke(this, EventArgs.Empty);

            ConnectedDevice.Dispose();
            ConnectedDevice = null;

            DeviceDisconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
