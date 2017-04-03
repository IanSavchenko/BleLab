using BleLab.Commands.Device;

namespace BleLab.Commands.Formatters.Device
{
    public class DisconnectDeviceFormatter : CommandFormatterBase<DisconnectDeviceCommand>
    {
        public override object OnEnqueued(DisconnectDeviceCommand command)
        {
            return null;
        }

        public override object OnDispatched(DisconnectDeviceCommand command)
        {
            return "Disconnecting from device...";
        }

        public override object OnExecuted(DisconnectDeviceCommand command)
        {
            switch (command.Status)
            {
                case CommandStatus.Succeeded:
                    return "Disconnected.";
                case CommandStatus.Exception:
                    return $"Exception while disconnecting: {command.Exception?.Message}";
                default:
                    return "Something went wrong...";
            }
        }
    }
}
