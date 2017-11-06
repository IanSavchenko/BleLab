using BleLab.Commands.Characteristic;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class ReadClientConfigDescriptorFormatter : CommandFormatterBase<ReadClientConfigDescriptorCommand>
    {
        public override object OnEnqueued(ReadClientConfigDescriptorCommand command)
        {
            return null;
        }

        public override object OnDispatched(ReadClientConfigDescriptorCommand command)
        {
            return $"Reading client config descriptor for characteristc '{command.CharacteristicInfo.Name}' ({command.CharacteristicInfo.Uuid})";
        }

        public override object OnExecuted(ReadClientConfigDescriptorCommand command)
        {
            switch (command.Status)
            {
                case CommandStatus.Succeeded:
                    return command.Descriptor.ToString();
                case CommandStatus.Exception:
                    return $"Exception: {command.Exception.Message}";
                default:
                    return null;
            }
        }
    }
}