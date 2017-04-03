using System;
using System.Threading.Tasks;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.ViewModels.Characteristic
{
    public class NotesViewModel : PropertyChangedBase
    {
        private readonly CharacteristicInfo _characteristicInfo;
        private readonly Lazy<InfoManager> _infoManagerLazy = new Lazy<InfoManager>(() => IoC.Get<InfoManager>());

        public NotesViewModel(CharacteristicInfo characteristicInfo)
        {
            _characteristicInfo = characteristicInfo;
            Text = _characteristicInfo.Notes;
        }

        public string Text { get; set; }

        public void OnTextLostFocus()
        {
            if (Text == _characteristicInfo.Notes)
                return;

            _characteristicInfo.Notes = Text;
            Task.Run(() => _infoManagerLazy.Value.SaveCharacteristic(_characteristicInfo));
        }
    }
}
