using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands;
using BleLab.Commands.Characteristic;
using BleLab.Model;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class NotificationsViewModel : PropertyChangedBase
    {
        private readonly CharacteristicInfo _characteristicInfo;
        private readonly CommandRunner _commandRunner;
        private bool _canChange = true;
        private bool _isSubscribed;

        public NotificationsViewModel(CharacteristicInfo characteristicInfo)
        {
            _commandRunner = IoC.Get<CommandRunner>();

            _characteristicInfo = characteristicInfo;
            Descriptors = new List<ClientDescriptorViewModel>();

            var properties = _characteristicInfo.Properties;
            if (properties.HasFlag(GattCharacteristicProperties.Indicate))
                Descriptors.Add(new ClientDescriptorViewModel(GattClientCharacteristicConfigurationDescriptorValue.Indicate, "Indicate"));
            
            if (properties.HasFlag(GattCharacteristicProperties.Notify))
                Descriptors.Add(new ClientDescriptorViewModel(GattClientCharacteristicConfigurationDescriptorValue.Notify, "Notify"));
        }

        public List<ClientDescriptorViewModel> Descriptors { get; }

        public bool CanChange
        {
            get { return _canChange; }
            set
            {
                if (value == _canChange) return;
                _canChange = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsSubscribed
        {
            get { return _isSubscribed; }
            set
            {
                if (value == _isSubscribed) return;
                _isSubscribed = value;
                NotifyOfPropertyChange();
            }
        }

        public async void Read()
        {
            try
            {
                CanChange = false;

                var result = await _commandRunner.Enqueue(new ReadClientConfigDescriptorCommand(_characteristicInfo)).GetTask();
                if (result.Status == CommandStatus.Succeeded)
                {
                    foreach (var descriptorViewModel in Descriptors)
                    {
                        descriptorViewModel.SetStateRead(result.Descriptor.HasFlag(descriptorViewModel.Descriptor));
                    }
                }
            }
            finally
            {
                CanChange = true;
            }
        }

        public async void Write()
        {
            var descriptor = GattClientCharacteristicConfigurationDescriptorValue.None;
            foreach (var descriptorViewModel in Descriptors)
            {
                if (descriptorViewModel.Enabled)
                    descriptor = descriptor | descriptorViewModel.Descriptor;
            }

            try
            {
                CanChange = false;

                var result = await _commandRunner.Enqueue(new WriteClientConfigDescriptorCommand(_characteristicInfo, descriptor)).GetTask();
                if (result.Status == CommandStatus.Succeeded)
                {
                    foreach (var descriptrorViewModel in Descriptors)
                    {
                        descriptrorViewModel.SetStateWritten();
                    }
                }
            }
            finally
            {
                CanChange = true;
            }
        }

        public async void Subscribe()
        {
            CanChange = false;
            try
            {
                var result = await _commandRunner.Enqueue(new SubscribeCommand(_characteristicInfo)).GetTask();
                if (result.Status == CommandStatus.Succeeded)
                    IsSubscribed = true;
            }
            finally
            {
                CanChange = true;
            }
            
        }

        public async void Unsubscribe()
        {
            CanChange = false;
            try
            {
                var result = await _commandRunner.Enqueue(new UnsubscribeCommand(_characteristicInfo)).GetTask();
                if (result.Status == CommandStatus.Succeeded)
                    IsSubscribed = false;
            }
            finally
            {
                CanChange = true;
            }
        }
    }
}
