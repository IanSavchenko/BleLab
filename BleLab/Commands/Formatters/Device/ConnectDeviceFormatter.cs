using BleLab.Commands.Device;

namespace BleLab.Commands.Formatters.Device
{
    public class ConnectDeviceFormatter : CommandFormatterBase<ConnectDeviceCommand>
    {
        public override object OnEnqueued(ConnectDeviceCommand command)
        {
            return null;
        }

        public override object OnDispatched(ConnectDeviceCommand command)
        {
            return $"Trying to connect to device '{command.DeviceInfo.DisplayName}' ({command.DeviceInfo.DeviceId})";
        }

        public override object OnExecuted(ConnectDeviceCommand command)
        {
            switch (command.Status)
            {
                case CommandStatus.Succeeded:
                    return $"Successfully connected to '{command.DeviceInfo.DisplayName}'.";
                case CommandStatus.Exception:
                    return $"Exception while connecting to device '{command.DeviceInfo.DisplayName}': {command.Exception.Message}";
                default:
                    return null;
            }
        }
    }
}
