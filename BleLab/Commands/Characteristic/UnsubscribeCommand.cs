using System.Threading.Tasks;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Characteristic
{
    public class UnsubscribeCommand : CharacteristicCommandBase
    {
        private readonly CharacteristicSubscriptionService _subscriptionService;

        public UnsubscribeCommand(CharacteristicInfo characteristicInfo) : base(characteristicInfo)
        {
            _subscriptionService = IoC.Get<CharacteristicSubscriptionService>();
        }

        public bool SubscriptionNotFound { get; private set; }

        protected override async Task DoExecuteAsync()
        {
            SubscriptionNotFound = !_subscriptionService.Unsubscribe(Characteristic);
        }
    }
}
