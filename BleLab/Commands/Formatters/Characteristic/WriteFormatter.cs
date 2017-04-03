using BleLab.Commands.Characteristic;
using BleLab.Utils;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class WriteFormatter : CommandFormatterBase<WriteBytesCommand>
    {
        public override object OnEnqueued(WriteBytesCommand command)
        {
            return null;
        }

        public override object OnDispatched(WriteBytesCommand command)
        {
            return $"WriteBytes {(command.WithoutResponse ? "WithoutResponce " : string.Empty)}{command.CharacteristicInfo.Uuid} {command.Bytes.AsString()}";
        }

        public override object OnExecuted(WriteBytesCommand command)
        {
            if (command.Status == CommandStatus.Succeeded)
                return "Successfull write";

            if (command.Status == CommandStatus.Unreachable)
                return "Device unreachable";

            if (command.Status == CommandStatus.Exception)
                return $"Exception when writing: {command.Exception.Message}";

            return null;
        }
    }
}