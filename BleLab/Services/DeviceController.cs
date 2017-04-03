using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using BleLab.Model;
using BleLab.Utils;

namespace BleLab.Services
{
    public class DeviceController
    {
        private readonly CharacteristicSubscriptionService _characteristicSubscriptionService;
        private readonly InfoManager _infoManager;

        public DeviceController(CharacteristicSubscriptionService characteristicSubscriptionService, InfoManager infoManager)
        {
            _characteristicSubscriptionService = characteristicSubscriptionService;
            _infoManager = infoManager;
        }

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
        }

        public void Disconnect()
        {
            if (ConnectedDevice == null)
                return;

            _characteristicSubscriptionService.DeviceDisconnected(ConnectedDevice);

            ConnectedDevice.Dispose();
            ConnectedDevice = null;
        }
    }
}
