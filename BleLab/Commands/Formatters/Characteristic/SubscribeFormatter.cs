using BleLab.Commands.Characteristic;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class SubscribeFormatter : CommandFormatterBase<SubscribeCommand>
    {
        public override object OnEnqueued(SubscribeCommand command)
        {
            return null;
        }

        public override object OnDispatched(SubscribeCommand command)
        {
            return $"Subscribing to characteristic '{command.CharacteristicInfo.Name}' ({command.CharacteristicInfo.Uuid})";
        }

        public override object OnExecuted(SubscribeCommand command)
        {
            if (command.Status == CommandStatus.Succeeded)
                return command.AlreadySubscribed ? "Already subscribed" : "Successfully subscribed";

            if (command.Status == CommandStatus.Exception)
                return "Exception while subscribing: " + command.Exception.Message;

            return null;
        }
    }
}
