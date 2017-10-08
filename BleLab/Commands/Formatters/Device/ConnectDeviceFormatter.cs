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
            return $"Trying to connect to device '{command.DeviceInfo.DeviceId}'";
        }

        public override object OnExecuted(ConnectDeviceCommand command)
        {
            switch (command.Status)
            {
                case CommandStatus.Succeeded:
                    return "Successfully connected.";
                case CommandStatus.Exception:
                    return $"Exception while connecting: {command.Exception.Message}";
                default:
                    return null;
            }
        }
    }
}
