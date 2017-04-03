using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Device
{
    public class ConnectDeviceCommand : CommandBase
    {
        private readonly DeviceController _deviceController;

        public ConnectDeviceCommand(DeviceInfo deviceInfo)
        {
            RunOnUiThread = true;

            DeviceInfo = deviceInfo;
            _deviceController = IoC.Get<DeviceController>();
        }
        
        public DeviceInfo DeviceInfo { get; }

        public BluetoothLEDevice Device { get; set; }

        protected override async Task DoExecuteAsync()
        {
            await _deviceController.Connect(DeviceInfo).ConfigureAwait(false);

            Device = _deviceController.ConnectedDevice;

            if (_deviceController.ConnectedDevice == null)
                Status = CommandStatus.Unreachable;
        }
    }
}
