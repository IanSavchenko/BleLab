using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using BleLab.Services;
using Caliburn.Micro;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BleLab.Views
{
    public sealed partial class DeviceShellView : UserControl
    {
        private readonly ApplicationState _applicationState;

        public DeviceShellView()
        {
            _applicationState = IoC.Get<ApplicationState>();
            this.InitializeComponent();

            HideConsole();
            IsConsoleExpanded = _applicationState.ConsoleExpanded;

            ConsoleRowDefinition.Height = new GridLength(_applicationState.ConsoleHeight);
            Splitter.ManipulationCompleted += SplitterOnManipulationCompleted;
        }

        public static readonly DependencyProperty IsConsoleExpandedProperty = DependencyProperty.Register(
            "IsConsoleExpanded", typeof(bool), typeof(DeviceShellView), new PropertyMetadata(default(bool), OnIsExpandedChanged));

        public bool IsConsoleExpanded
        {
            get => (bool)GetValue(IsConsoleExpandedProperty);
            set => SetValue(IsConsoleExpandedProperty, value);
        }

        private static void OnIsExpandedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var view = (DeviceShellView)dependencyObject;
            var isExpanded = (bool)args.NewValue;
            if (isExpanded)
            {
                view.ExpandConsole();
            }
            else
            {
                view.HideConsole();
            }

            view._applicationState.ConsoleExpanded = isExpanded;
        }
        
        private void SplitterOnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs manipulationCompletedRoutedEventArgs)
        {
            _applicationState.ConsoleHeight = (int)ConsoleRowDefinition.Height.Value;
        }

        private void ExpandConsole_OnClick(object sender, RoutedEventArgs e)
        {
            IsConsoleExpanded = true;
        }

        private void HideConsole_OnClick(object sender, RoutedEventArgs e)
        {
            IsConsoleExpanded = false;
        }
        
        private void ExpandConsole()
        {
            ConsoleRowDefinition.MaxHeight = Double.PositiveInfinity;
            ConsoleRowDefinition.MinHeight = 120;
            Splitter.Visibility = Visibility.Visible;
            ExpandConsoleButton.Visibility = Visibility.Collapsed;
            HideConsoleButton.Visibility = Visibility.Visible;
        }

        private void HideConsole()
        {
            Splitter.Visibility = Visibility.Collapsed;
            ConsoleRowDefinition.MinHeight = 30;
            ConsoleRowDefinition.MaxHeight = 30;
            ExpandConsoleButton.Visibility = Visibility.Visible;
            HideConsoleButton.Visibility = Visibility.Collapsed;
        }
    }
}
