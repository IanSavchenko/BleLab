using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Device
{
    public class ListDevicesCommand : CommandBase
    {
        private readonly InfoManager _manager;

        public ListDevicesCommand()
        {
            _manager = IoC.Get<InfoManager>();
        }

        public ICollection<DeviceInfo> Devices { get; private set; }

        protected override async Task DoExecuteAsync()
        {
            var pairedDevices = await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector());
            Devices = await _manager.GetAllPairedDevicesInfo(pairedDevices).ConfigureAwait(false);
        }
    }
}
