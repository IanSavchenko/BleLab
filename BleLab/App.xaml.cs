using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using BleLab.Commands;
using BleLab.Commands.Formatters.Characteristic;
using BleLab.Commands.Formatters.Device;
using BleLab.GattInformation;
using BleLab.Messages;
using BleLab.Services;
using BleLab.ViewModels;
using Caliburn.Micro;

#if !DEBUG
using Microsoft.HockeyApp;
#endif

namespace BleLab
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : CaliburnApplication
    {
        private WinRTContainer _container;
        private IEventAggregator _eventAggregator;

        public App()
        {
            InitializeComponent();
#if !DEBUG
            HockeyClient.Current.Configure("961a62efc1d54798b7b2425e61abf7b2");
#endif
        }

        protected override void Configure()
        {
#if DEBUG
            LogManager.GetLog = type => new CaliburnDebugLogger(type);
#endif

            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container.Singleton<GattInformationProvider>().GetInstance<GattInformationProvider>().Initialize();

            _container.Singleton<InfoManager>();
            _container.Singleton<CommandRunner>();
            _container.Singleton<EventTracer>();
            _container.Singleton<CharacteristicSubscriptionService>();
            _container.Singleton<DeviceController>();

            _container.Singleton<CommandPanelViewModel>();

            _container
                .PerRequest<MainViewModel>()
                .PerRequest<DeviceShellViewModel>()
                .PerRequest<SettingsViewModel>()
                .PerRequest<SelectDeviceViewModel>()
                .PerRequest<AboutViewModel>();

            _container
                .PerRequest<ICommandFormatter, ReadFormatter>()
                .PerRequest<ICommandFormatter, WriteFormatter>()
                .PerRequest<ICommandFormatter, ReadClientConfigDescriptorFormatter>()
                .PerRequest<ICommandFormatter, WriteClientConfigDescriptorFormatter>()
                .PerRequest<ICommandFormatter, SubscribeFormatter>()
                .PerRequest<ICommandFormatter, UnsubscribeFormatter>()
                .PerRequest<ICommandFormatter, ConnectDeviceFormatter>()
                .PerRequest<ICommandFormatter, DisconnectDeviceFormatter>();

            var tracer = _container.GetInstance<EventTracer>();

            _eventAggregator = _container.GetInstance<IEventAggregator>();
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;
            
            DisplayRootViewFor<MainViewModel>();

            // It's kinda of weird having to use the event aggregator to pass 
            // a value to ShellViewModel, could be an argument for allowing
            // parameters or launch arguments

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                _eventAggregator.PublishOnUIThread(new ResumeStateMessage());
            }
        }

        protected override void OnSuspending(object sender, SuspendingEventArgs e)
        {
            _eventAggregator.PublishOnUIThread(new SuspendStateMessage(e.SuspendingOperation));
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }


    class CaliburnDebugLogger : Caliburn.Micro.ILog
    {
#region Fields
        private readonly Type _type;
#endregion

#region Constructors
        public CaliburnDebugLogger(Type type)
        {
            _type = type;
        }
#endregion

#region Helper Methods
        private string CreateLogMessage(string format, params object[] args)
        {
            return string.Format("[{0}] {1}",
                                 DateTime.Now.ToString("o"),
                                 string.Format(format, args));
        }
#endregion

#region ILog Members
        public void Error(Exception exception)
        {
            Debug.WriteLine(CreateLogMessage(exception.ToString()), "ERROR");
        }
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "INFO");
        }
        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine(CreateLogMessage(format, args), "WARN");
        }
#endregion
    }
}
