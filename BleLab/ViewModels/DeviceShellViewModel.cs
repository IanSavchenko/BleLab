using Caliburn.Micro;

namespace BleLab.ViewModels
{
    internal class DeviceShellViewModel : Conductor<object>
    {
        private readonly SelectDeviceViewModel _selectDeviceViewModel;
        private object _cachedView;

        public DeviceShellViewModel(SelectDeviceViewModel selectDeviceViewModel, CommandPanelViewModel commandPanel)
        {
            CommandPanel = commandPanel;
            _selectDeviceViewModel = selectDeviceViewModel;
            _selectDeviceViewModel.DeviceActivated += SelectDeviceViewModelOnDeviceActivated;
            ActivateItem(_selectDeviceViewModel);
        }

        public CommandPanelViewModel CommandPanel { get; }

        public override void DeactivateItem(object item, bool close)
        { 
            base.DeactivateItem(item, close);
            if (close)
                ActivateItem(_selectDeviceViewModel);
        }

        protected override void OnViewAttached(object view, object context)
        {
            _cachedView = view;
            base.OnViewAttached(view, context);
        }

        public override object GetView(object context = null)
        {
            return _cachedView;
        }

        private void SelectDeviceViewModelOnDeviceActivated(DeviceInfoViewModel deviceInfoViewModel)
        {
            var deviceViewModel = new DeviceViewModel(deviceInfoViewModel.Info);
            ActivateItem(deviceViewModel);
        }
    }
}
