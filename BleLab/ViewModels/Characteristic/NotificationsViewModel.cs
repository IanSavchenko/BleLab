using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands;
using BleLab.Commands.Characteristic;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class NotificationsViewModel : PropertyChangedBase
    {
        private readonly CharacteristicInfo _characteristicInfo;
        private readonly Lazy<InfoManager> _infoManagerLazy = new Lazy<InfoManager>(() => IoC.Get<InfoManager>());
        private readonly CommandRunner _commandRunner;
        private bool _canChange = true;
        private bool _isSubscribed;
        private BytesDisplayFormatViewModel _selectedDisplayFormat;

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

            _selectedDisplayFormat = DisplayFormats.FirstOrDefault(x => x.Model == characteristicInfo.NotificationDisplayFormat) ?? DisplayFormats[0];
            if (_selectedDisplayFormat.Model != characteristicInfo.NotificationDisplayFormat)
            {
                _characteristicInfo.NotificationDisplayFormat = _selectedDisplayFormat.Model;
                Task.Run(() => _infoManagerLazy.Value.SaveCharacteristic(_characteristicInfo));
            }
        }

        public List<ClientDescriptorViewModel> Descriptors { get; }

        public bool CanChange
        {
            get => _canChange;
            set
            {
                if (value == _canChange) return;
                _canChange = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsSubscribed
        {
            get => _isSubscribed;
            set
            {
                if (value == _isSubscribed) return;
                _isSubscribed = value;
                NotifyOfPropertyChange();
            }
        }

        public List<BytesDisplayFormatViewModel> DisplayFormats { get; } =
            new[] { BytesDisplayFormat.Decimal, BytesDisplayFormat.Hexadecimal, BytesDisplayFormat.Utf8, BytesDisplayFormat.Utf16, BytesDisplayFormat.Utf16Be }
                .Select(x => new BytesDisplayFormatViewModel(x))
                .ToList();

        public BytesDisplayFormatViewModel SelectedDisplayFormat
        {
            get => _selectedDisplayFormat;
            set
            {
                if (value == _selectedDisplayFormat) return;
                _selectedDisplayFormat = value;
                NotifyOfPropertyChange();

                _characteristicInfo.NotificationDisplayFormat = _selectedDisplayFormat.Model;
                Task.Run(() => _infoManagerLazy.Value.SaveCharacteristic(_characteristicInfo));
            }
        }

        public async void Read()
        {
            try
            {
                CanChange = false;

                var result = await _commandRunner.Enqueue(new ReadClientConfigDescriptorCommand(_characteristicInfo)).AsTask().ConfigureAwait(true);
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

                var result = await _commandRunner.Enqueue(new WriteClientConfigDescriptorCommand(_characteristicInfo, descriptor)).AsTask().ConfigureAwait(true);
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
                var result = await _commandRunner.Enqueue(new SubscribeCommand(_characteristicInfo)).AsTask().ConfigureAwait(true);
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
                var result = await _commandRunner.Enqueue(new UnsubscribeCommand(_characteristicInfo)).AsTask().ConfigureAwait(true);
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
