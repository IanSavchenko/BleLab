using System.Threading.Tasks;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Device
{
    public class DisconnectDeviceCommand : CommandBase
    {
        private readonly DeviceController _deviceController;

        public DisconnectDeviceCommand()
        {
            _deviceController = IoC.Get<DeviceController>();
        }

        protected override async Task DoExecuteAsync()
        {
            _deviceController.Disconnect();
        }
    }
}
