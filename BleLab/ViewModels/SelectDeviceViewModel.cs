using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Radios;
using Windows.UI.Xaml.Controls;
using BleLab.Commands;
using BleLab.Commands.Device;
using BleLab.Messages;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    internal class SelectDeviceViewModel : Screen, IHandle<DeviceFavouriteStateChangedMessage>, IHandle<DeviceForgottenMessage>
    {
        private readonly CommandRunner _commandRunner;
        private bool _noDevicesPaired;
        private bool _noBluetooth;
        private bool _bluetoothDisabled;

        public SelectDeviceViewModel(CommandRunner commandRunner, IEventAggregator eventAggregator)
        {
            _commandRunner = commandRunner;
            eventAggregator.Subscribe(this);
        }

        public event Action<DeviceInfoViewModel> DeviceActivated;  

        public ObservableCollection<DeviceInfoViewModel> Devices { get; } = new ObservableCollection<DeviceInfoViewModel>();

        public bool HasError => NoDevicesPaired || NoBluetooth || BluetoothDisabled;

        public bool NoBluetooth
        {
            get { return _noBluetooth; }
            set
            {
                if (value == _noBluetooth) return;
                _noBluetooth = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(HasError));
            }
        }

        public bool BluetoothDisabled
        {
            get { return _bluetoothDisabled; }
            set
            {
                if (value == _bluetoothDisabled) return;
                _bluetoothDisabled = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(HasError));
            }
        }

        public bool NoDevicesPaired
        {
            get { return _noDevicesPaired; }
            set
            {
                if (value == _noDevicesPaired) return;
                _noDevicesPaired = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(HasError));
            }
        }
        
        public void DeviceClicked(ItemClickEventArgs eventArgs)
        {
            var device = eventArgs.ClickedItem as DeviceInfoViewModel;
            if (device == null)
                return;

            DeviceActivated?.Invoke(device);
        }

        public void Handle(DeviceFavouriteStateChangedMessage message)
        {
            SortAndUpdateDisplayedCollection();
        }

        public void Handle(DeviceForgottenMessage message)
        {
            RefreshDevicesList();
        }

        public void OpenBluetoothSettings()
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:bluetooth"));
        }

        protected override void OnActivate()
        {
            RefreshDevicesList();
        }

        public async void RefreshDevicesList()
        {
            NoBluetooth = false;
            BluetoothDisabled = false;
            NoDevicesPaired = false;

            Devices.Clear();

            var result = await _commandRunner.Enqueue(new ListDevicesCommand()).AsTask();
            if (result.Status != CommandStatus.Succeeded)
            {
                DiagnoseProblem();
                return;
            }

            foreach (var deviceInfoViewModel in result.Devices.Select(t => new DeviceInfoViewModel(t)))
            {
                Devices.Add(deviceInfoViewModel);
            }

            SortAndUpdateDisplayedCollection();

            if (Devices.Count == 0)
                DiagnoseProblem();
        }

        private void SortAndUpdateDisplayedCollection()
        {
            var devicesOrdered = Devices.OrderByDescending(x => x.IsFavourite ? 1 : 0).ThenBy(x => x.Name).ToList();

            for (var i = 0; i < devicesOrdered.Count; i++)
            {
                var currentIndex = Devices.IndexOf(devicesOrdered[i]);
                if (currentIndex != i)
                    Devices.Move(currentIndex, i);
            }
        }
        
        private async void DiagnoseProblem()
        {
            if (await GetBluetoothIsSupportedAsync() == false)
            {
                NoBluetooth = true;
                return;
            }

            if (await GetBluetoothIsEnabledAsync() == false)
            {
                BluetoothDisabled = true;
                return;
            }

            NoDevicesPaired = Devices.Count == 0;
        }

        private static async Task<bool> GetBluetoothIsSupportedAsync()
        {
            var radios = await Radio.GetRadiosAsync();
            return radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth) != null;
        }

        private static async Task<bool> GetBluetoothIsEnabledAsync()
        {
            var radios = await Radio.GetRadiosAsync();
            var bluetoothRadio = radios.FirstOrDefault(radio => radio.Kind == RadioKind.Bluetooth);
            return bluetoothRadio != null && bluetoothRadio.State == RadioState.On;
        }
    }
}
