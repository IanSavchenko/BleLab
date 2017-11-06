using BleLab.Commands.Characteristic;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class WriteClientConfigDescriptorFormatter : CommandFormatterBase<WriteClientConfigDescriptorCommand>
    {
        public override object OnEnqueued(WriteClientConfigDescriptorCommand command)
        {
            return null;
        }

        public override object OnDispatched(WriteClientConfigDescriptorCommand command)
        {
            return $"Writing client config descriptor '{command.Descriptor}' to characteristic '{command.CharacteristicInfo.Name}' ({command.CharacteristicInfo.Uuid})";
        }

        public override object OnExecuted(WriteClientConfigDescriptorCommand command)
        {
            switch (command.Status)
            {
                case CommandStatus.Succeeded:
                    return "Successfull write.";
                case CommandStatus.Exception:
                    return $"Exception: {command.Exception.Message}";
                default:
                    return null;
            }
        }
    }
}
