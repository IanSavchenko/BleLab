using System;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using BleLab.Messages;
using BleLab.Model;
using BleLab.Services;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    internal class DeviceInfoViewModel : PropertyChangedBase
    {
        private readonly Lazy<IEventAggregator> _eventAggregator = new Lazy<IEventAggregator>(() => IoC.Get<IEventAggregator>());

        public DeviceInfoViewModel(DeviceInfo deviceInfo)
        {
            Info = deviceInfo;
            Glyph = new BitmapImage();
            Glyph.SetSource(Info.Glyph);
        }

        public string Name => Info.GivenName ?? Info.Name;

        public bool IsNew => Info.IsNew;

        public bool CanForget => !IsNew;

        public bool IsFavourite
        {
            get { return Info.IsFavourite; }

            set
            {
                Info.IsFavourite = value;
                Info.SaveAsync();

                _eventAggregator.Value.PublishOnUIThreadAsync(new DeviceFavouriteStateChangedMessage(Info));
            }
        }

        public DeviceInfo Info { get; }

        public BitmapImage Glyph { get; }

        public async void Forget()
        {
            var md = new MessageDialog("Are you sure you want to forget device? All saved data will be lost.", "Confirmation");
            md.Commands.Add(new UICommand("Yes, forget", x => DoForget()));
            md.Commands.Add(new UICommand("No-no", x => { }));
            md.DefaultCommandIndex = 1;

            await md.ShowAsync();
        }

        private void DoForget()
        {
            var infoManager = IoC.Get<InfoManager>();
            infoManager.ForgetDevice(Info);

            _eventAggregator.Value.PublishOnUIThreadAsync(new DeviceForgottenMessage(Info));
        }
    }
}
