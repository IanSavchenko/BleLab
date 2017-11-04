using System.Threading.Tasks;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.Commands.Characteristic
{
    public class SubscribeCommand : CharacteristicCommandBase
    {
        private readonly CharacteristicSubscriptionService _subscriptionService;

        public SubscribeCommand(CharacteristicInfo characteristicInfo) : base(characteristicInfo)
        {
            _subscriptionService = IoC.Get<CharacteristicSubscriptionService>();
        }

        public bool AlreadySubscribed { get; private set; }

        protected override async Task DoExecuteAsync()
        {
            AlreadySubscribed = !_subscriptionService.Subscribe(CharacteristicInfo);
        }
    }
}
