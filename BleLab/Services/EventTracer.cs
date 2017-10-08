using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands;
using BleLab.Utils;
using BleLab.ViewModels;

namespace BleLab.Services
{
    public class EventTracer
    {
        private readonly CommandRunner _commandRunner;
        private readonly CommandPanelViewModel _commandPanelViewModel;
        private readonly CharacteristicSubscriptionService _subscriptionService;
        private readonly Dictionary<Type, ICommandFormatter> _formatters;

        public EventTracer(
            CommandRunner commandRunner, 
            CommandPanelViewModel commandPanelViewModel, 
            IEnumerable<ICommandFormatter> formatters, 
            CharacteristicSubscriptionService subscriptionService)
        {
            _commandRunner = commandRunner;
            _commandPanelViewModel = commandPanelViewModel;
            _subscriptionService = subscriptionService;
            _formatters = formatters.ToDictionary(formatter => formatter.CommandType);

            _commandRunner.Executed += RunnerOnExecuted;
            _commandRunner.Dispatched += RunnerOnDispatched;
            _commandRunner.Enqueued += RunnerOnEnqueued;

            subscriptionService.ValueChanged += SubscriptionServiceOnValueChanged;
        }

        private void Display(object formattedInput)
        {
            if (formattedInput == null)
                return;

            _commandPanelViewModel.AddMessage(formattedInput);
        }

        private void RunnerOnEnqueued(object sender, CommandRunnerEvent commandRunnerEvent)
        {
            if (commandRunnerEvent.Command.HideInConsole)
                return;

            if (!_formatters.TryGetValue(commandRunnerEvent.Command.GetType(), out ICommandFormatter formatter))
                return;

            Display(formatter.OnEnqueued(commandRunnerEvent.Command));
        }

        private void RunnerOnDispatched(object sender, CommandRunnerEvent commandRunnerEvent)
        {
            if (commandRunnerEvent.Command.HideInConsole)
                return;

            if (!_formatters.TryGetValue(commandRunnerEvent.Command.GetType(), out ICommandFormatter formatter))
                return;

            Display(formatter.OnDispatched(commandRunnerEvent.Command));
        }

        private void RunnerOnExecuted(object sender, CommandRunnerEvent commandRunnerEvent)
        {
            if (commandRunnerEvent.Command.HideInConsole)
                return;

            if (!_formatters.TryGetValue(commandRunnerEvent.Command.GetType(), out ICommandFormatter formatter))
                return;

            Display(formatter.OnExecuted(commandRunnerEvent.Command));
        }
        
        private void SubscriptionServiceOnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Display($"Event in characteristic {sender.Uuid}: {args.CharacteristicValue.ToArraySafe().AsString()}");
        }
    }
}
