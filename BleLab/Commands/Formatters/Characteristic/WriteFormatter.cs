using System;
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
            return $"WriteBytes {(command.WithoutResponse ? $"'{nameof(WriteBytesCommand.WithoutResponse)}' " : string.Empty)}to characteristic '{command.CharacteristicInfo.Name}' ({command.CharacteristicInfo.Uuid}){Environment.NewLine}{command.Bytes.AsString(command.BytesDisplayFormat)}";
        }

        public override object OnExecuted(WriteBytesCommand command)
        {
            if (command.Status == CommandStatus.Succeeded)
                return "Successfull write.";

            if (command.Status == CommandStatus.Exception)
                return $"Exception when writing: {command.Exception.Message}";

            return null;
        }
    }
}