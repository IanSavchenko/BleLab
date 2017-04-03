using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BleLab.Model;
using BleLab.Utils;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class ServiceViewModel : PropertyChangedBase
    {
        private readonly ServiceInfo _serviceInfo;

        public ServiceViewModel(ServiceInfo serviceInfo)
        {
            _serviceInfo = serviceInfo;
            ReadMoreCommand = new DelegateCommand(o => true, o => ReadMore());
        }

        public ObservableCollection<CharacteristicInfoViewModel> Characteristics { get; } = new ObservableCollection<CharacteristicInfoViewModel>();
        
        public string Uuid => _serviceInfo.Uuid.ToString("D").ToUpper();

        public string Name
        {
            get { return _serviceInfo.Name; }

            set
            {
                _serviceInfo.Name = value;
                _serviceInfo.SaveAsync();
            }
        }

        public string Description
        {
            get { return _serviceInfo.Description; }
            set
            {
                _serviceInfo.Description = value;
                _serviceInfo.SaveAsync();
            }
        }

        public bool IsFavourite
        {
            get { return _serviceInfo.IsFavourite; }

            set
            {
                _serviceInfo.IsFavourite = value;
                _serviceInfo.SaveAsync();
            }
        }

        public Uri ReadMoreLink
        {
            get
            {
                if (_serviceInfo.WebLink != null)
                    return new Uri(_serviceInfo.WebLink);

                return null;
            }
        }

        public bool IsReadMoreVisible => ReadMoreLink != null;

        public ICommand ReadMoreCommand { get; }



        public async void ReadMore()
        {
            if (ReadMoreLink == null)
                return;

            var success = await Windows.System.Launcher.LaunchUriAsync(ReadMoreLink);
        }
    }
}
