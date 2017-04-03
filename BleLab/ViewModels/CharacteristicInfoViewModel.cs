using BleLab.Model;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class CharacteristicInfoViewModel : PropertyChangedBase
    {
        private readonly CharacteristicInfo _characteristicInfo;

        public CharacteristicInfoViewModel(CharacteristicInfo characteristicInfo)
        {
            _characteristicInfo = characteristicInfo;
        }

        public CharacteristicInfo Info => _characteristicInfo;

        public bool IsFavourite
        {
            get { return _characteristicInfo.IsFavourite; }

            set
            {
                _characteristicInfo.IsFavourite = value;
                _characteristicInfo.SaveAsync();
            }
        }

        public string Uuid => _characteristicInfo.Uuid.ToString("D").ToUpper();

        public string Description => _characteristicInfo.Description;

        public CharacteristicInfo Model => _characteristicInfo;

        public string Name
        {
            get
            {
                if (_characteristicInfo.Name != null)
                    return _characteristicInfo.Name;

                return "<Unknown characteristic>";
            }
        }
    }
}