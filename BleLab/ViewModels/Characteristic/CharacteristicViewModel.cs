using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.System;
using BleLab.Messages;
using BleLab.Model;
using Caliburn.Micro;
using Action = Caliburn.Micro.Action;
using CharacteristicView = BleLab.Views.Characteristic.CharacteristicView;

namespace BleLab.ViewModels.Characteristic
{
    public class CharacteristicViewModel : Screen
    {
        private readonly Lazy<IEventAggregator> _eventAggregator = new Lazy<IEventAggregator>(() => IoC.Get<IEventAggregator>());
        private readonly CharacteristicInfo _characteristicInfo;
        private object _cachedView;

        public CharacteristicViewModel(CharacteristicInfo characteristicInfo)
        {
            _characteristicInfo = characteristicInfo;
            Operations = new List<PropertyChangedBase>();

            if (CanRead)
                Operations.Add(new ReadViewModel(_characteristicInfo));

            if (CanWrite)
                Operations.Add(new WriteViewModel(_characteristicInfo));

            if (CanNotify)
                Operations.Add(new NotificationsViewModel(_characteristicInfo));

            Operations.Add(new NotesViewModel(_characteristicInfo));
        }

        public CharacteristicInfo Info => _characteristicInfo;

        public string Uuid => _characteristicInfo.Uuid.ToString("D").ToUpper();

        public string Name
        {
            get { return _characteristicInfo.Name; }
            set
            {
                _characteristicInfo.Name = value;
                _characteristicInfo.SaveAsync();

                _eventAggregator.Value.PublishOnUIThreadAsync(new CharacteristicRenamedMessage(_characteristicInfo));
            }
        }

        public bool IsFavourite
        {
            get { return _characteristicInfo.IsFavourite; }

            set
            {
                _characteristicInfo.IsFavourite = value;
                _characteristicInfo.SaveAsync();
            }
        }

        public string Description
        {
            get { return _characteristicInfo.Description; }
            set
            {
                _characteristicInfo.Description = value;
                _characteristicInfo.SaveAsync();
            }
        }

        public string Properties => _characteristicInfo.Properties.ToString();

        public ushort AttributeHandle => _characteristicInfo.AttributeHandle;

        public string Protection => _characteristicInfo.ProtectionLevel.ToString();

        public List<PropertyChangedBase> Operations { get; private set; }

        public bool CanRead => _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.Read);

        public bool CanWrite => _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.Write) 
            || _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);

        public bool CanNotify => _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.Notify)
            || _characteristicInfo.Properties.HasFlag(GattCharacteristicProperties.Indicate);

        public bool ReadMoreVisible => !string.IsNullOrWhiteSpace(_characteristicInfo.WebLink);

        public async void ReadMore()
        {
            var result = await Launcher.LaunchUriAsync(new Uri(_characteristicInfo.WebLink));
        }

        public override object GetView(object context = null)
        {
            return _cachedView;
        }

        protected override void OnViewAttached(object view, object context)
        {
            _cachedView = view;
        }

        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            var view = _cachedView as CharacteristicView;
            if (view == null)
                return;

            // fix for Caliburn's issue when conventions are not applied when cached view is activated. ToDo: report or fix
            Action.SetTarget(view, null);
            Action.SetTarget(view, this);
        }
    }
}
