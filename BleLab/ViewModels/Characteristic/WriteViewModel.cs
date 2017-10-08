using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using BleLab.Commands;
using BleLab.Commands.Characteristic;
using BleLab.Model;
using BleLab.Services;
using BleLab.Utils;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class WriteViewModel : PropertyChangedBase
    {
        private readonly Lazy<InfoManager> _infoManagerLazy = new Lazy<InfoManager>(() => IoC.Get<InfoManager>());
        private readonly CharacteristicInfo _characteristicInfo;
        private readonly CommandRunner _commandRunner;
        private bool _writeWithoutResponce;
        private bool _canWrite;
        private BytesDisplayFormatViewModel _selectedDisplayFormat;
        private string _bytesString;
        private bool _parsedOk;

        public WriteViewModel(CharacteristicInfo characteristicInfo)
        {
            _characteristicInfo = characteristicInfo;
            _commandRunner = IoC.Get<CommandRunner>();
            CanWrite = true;

            _selectedDisplayFormat = DisplayFormats.FirstOrDefault(x => x.Model == characteristicInfo.WriteDisplayFormat) ?? DisplayFormats[0];
        }

        public bool CanWriteBothWays => _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.Write | GattCharacteristicProperties.WriteWithoutResponse);

        public bool WriteWithoutResponce
        {
            get
            {
                if (!CanWriteBothWays)
                {
                    // constant
                    return _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
                }

                return _writeWithoutResponce;
            }

            set
            {
                _writeWithoutResponce = value;
            }
        }

        public bool CanWrite
        {
            get { return _canWrite && ParsedOk; }
            set
            {
                if (value == _canWrite) return;
                _canWrite = value;
                NotifyOfPropertyChange();
            }
        }

        public string BytesString
        {
            get { return _bytesString; }
            set
            {
                if (value == _bytesString) return;
                _bytesString = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ParsingStatus));
            }
        }

        public List<BytesDisplayFormatViewModel> DisplayFormats { get; } =
            new[] { BytesDisplayFormat.Auto, BytesDisplayFormat.Decimal, BytesDisplayFormat.Hexadecimal, BytesDisplayFormat.Utf8, BytesDisplayFormat.Utf16, BytesDisplayFormat.Utf16Be }
            .Select(x => new BytesDisplayFormatViewModel(x))
            .ToList();

        public BytesDisplayFormatViewModel SelectedDisplayFormat
        {
            get { return _selectedDisplayFormat; }
            set
            {
                if (value == _selectedDisplayFormat) return;
                _selectedDisplayFormat = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(ParsingStatus));
                
                _characteristicInfo.WriteDisplayFormat = _selectedDisplayFormat.Model;
                Task.Run(() => _infoManagerLazy.Value.SaveCharacteristic(_characteristicInfo));
            }
        }

        public string ParsingStatus
        {
            get
            {
                ParsedOk = false;
                var sb = new StringBuilder();

                var format = SelectedDisplayFormat.Model;
                if (format == BytesDisplayFormat.Auto)
                {
                    format = BytesFormatting.DetectFormatAuto(BytesString);
                    if (format == BytesDisplayFormat.None)
                        return string.Empty;

                    sb.Append($"{format.AsString()}. ");
                }

                byte[] bytes;

                try
                {
                    bytes = BytesFormatting.TryParse(BytesString, format);
                }
                catch (Exception ex)
                {
                    sb.Append(ex.Message);
                    return sb.ToString();
                }

                if (bytes == null)
                    return string.Empty;

                ParsedOk = true;
                sb.Append("Bytes length: " + bytes.Length);

                if (bytes.Length > 20)
                {
                    sb.AppendLine();
                    sb.Append("Writing more than 20 bytes is not supported by the protocol. But you can try...");
                }

                return sb.ToString();
            }
        }

        public bool ParsedOk
        {
            get { return _parsedOk; }
            set
            {
                if (value == _parsedOk) return;
                _parsedOk = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(CanWrite));
            }
        }

        public async void Write()
        {
            var format = SelectedDisplayFormat.Model;
            if (format == BytesDisplayFormat.Auto)
                format = BytesFormatting.DetectFormatAuto(BytesString);

            Debug.Assert(format != BytesDisplayFormat.None);
            Debug.Assert(format != BytesDisplayFormat.Auto);

            var bytes = BytesFormatting.TryParse(BytesString, format);
            if (bytes == null)
                return;

            try
            {
                CanWrite = false;
                var result = await _commandRunner.Enqueue(new WriteBytesCommand(_characteristicInfo, bytes, WriteWithoutResponce)).GetCompletedTask();
            }
            finally
            {
                CanWrite = true;
            }
        }
    }
}
