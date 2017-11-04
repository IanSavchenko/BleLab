using BleLab.Commands.Characteristic;
using BleLab.Utils;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class ReadFormatter : CommandFormatterBase<ReadBytesCommand>
    {
        public override object OnEnqueued(ReadBytesCommand command)
        {
            return null;
        }

        public override object OnDispatched(ReadBytesCommand command)
        {
            return $"ReadBytes {command.CacheMode} from characteristic {command.CharacteristicInfo.Uuid}.";
        }

        public override object OnExecuted(ReadBytesCommand command)
        {
            if (command.Status == CommandStatus.Succeeded)
                return $"{command.Bytes.AsString(command.BytesDisplayFormat)}";

            if (command.Status == CommandStatus.Exception)
                return $"Exception while reading data: {command.Exception.Message}";

            return null;
        }
    }
}