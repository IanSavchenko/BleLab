using BleLab.Model;

namespace BleLab.ViewModels
{
    public class BytesDisplayFormatViewModel
    {
        private readonly BytesDisplayFormat _displayFormat;

        public BytesDisplayFormatViewModel(BytesDisplayFormat displayFormat)
        {
            _displayFormat = displayFormat;
        }

        public BytesDisplayFormat Model => _displayFormat;

        public string AsString => _displayFormat.AsString();
    }
}
