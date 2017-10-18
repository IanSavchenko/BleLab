using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using BleLab.Commands;
using BleLab.Commands.Characteristic;
using BleLab.Model;
using BleLab.Services;
using BleLab.Utils;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class ReadViewModel : PropertyChangedBase
    {
        private readonly CharacteristicInfo _characteristicInfo;
        private readonly Lazy<InfoManager> _infoManagerLazy = new Lazy<InfoManager>(() => IoC.Get<InfoManager>());
        private readonly CommandRunner _commandRunner;
        private byte[] _readValue;
        private string _readSource;
        private bool _canRead;
        private BytesDisplayFormatViewModel _selectedDisplayFormat;

        public ReadViewModel(CharacteristicInfo characteristicInfo)
        {
            _commandRunner = IoC.Get<CommandRunner>();
            _characteristicInfo = characteristicInfo;
            _selectedDisplayFormat = DisplayFormats.FirstOrDefault(x => x.Model == _characteristicInfo.ReadDisplayFormat) ?? DisplayFormats[0];

            DoReadValue(cached: true, hideInConsole: true);
        }

        public byte[] ReadValue
        {
            get => _readValue;
            set
            {
                if (Equals(value, _readValue)) return;

                _readValue = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ReadValueString));
            }
        }

        public bool FromCache { get; set; }

        public string ReadValueString => ReadValue.AsString(SelectedDisplayFormat.Model);

        public string ReadSource
        {
            get => _readSource;
            set
            {
                if (value == _readSource) return;
                _readSource = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanRead
        {
            get => _canRead;
            set
            {
                if (value == _canRead) return;
                _canRead = value;
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
                NotifyOfPropertyChange(nameof(ReadValueString));

                _characteristicInfo.ReadDisplayFormat = _selectedDisplayFormat.Model;
                Task.Run(() => _infoManagerLazy.Value.SaveCharacteristic(_characteristicInfo));
            }
        }
        
        public async void Read()
        {
            await DoReadValue(FromCache).ConfigureAwait(true);
        }

        private async Task DoReadValue(bool cached, bool hideInConsole = false)
        {
            try
            {
                CanRead = false;
                var result = await _commandRunner
                    .Enqueue(new ReadBytesCommand(_characteristicInfo, cached ? BluetoothCacheMode.Cached : BluetoothCacheMode.Uncached) { HideInConsole = hideInConsole })
                    .AsTask().ConfigureAwait(true);

                if (result.Status != CommandStatus.Succeeded)
                {
                    ReadValue = null;
                    ReadSource = "Operation unsuccessful";
                    return;
                }

                ReadValue = result.Bytes;

                if (cached)
                    ReadSource = "From system cache";
                else
                    ReadSource = DateTime.Now.ToString("T");
            }
            finally
            {
                CanRead = true;
            }
        }
    }
}
