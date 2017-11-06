using BleLab.Commands.Characteristic;

namespace BleLab.Commands.Formatters.Characteristic
{
    public class UnsubscribeFormatter : CommandFormatterBase<UnsubscribeCommand>
    {
        public override object OnEnqueued(UnsubscribeCommand command)
        {
            return null;
        }

        public override object OnDispatched(UnsubscribeCommand command)
        {
            return $"Unsubscribing from characteristic '{command.CharacteristicInfo.Name}' ({command.CharacteristicInfo.Uuid})";
        }

        public override object OnExecuted(UnsubscribeCommand command)
        {
            if (command.Status == CommandStatus.Succeeded)
                return command.SubscriptionNotFound ? "Subscription not found." : "Unsubscribed";

            if (command.Status == CommandStatus.Exception)
                return "Exception while unsubscribing: " + command.Exception.Message;

            return null;
        }
    }
}
