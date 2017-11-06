using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using BleLab.Services;
using BleLab.ViewModels;
using Caliburn.Micro;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BleLab.Views
{
    public sealed partial class DeviceView : UserControl
    {
        private readonly ApplicationState _applicationState;

        public DeviceView()
        {
            _applicationState = IoC.Get<ApplicationState>();
            this.InitializeComponent();

            HidePane();
            
            PaneColumnDefinition.Width = new GridLength(_applicationState.CharacteristicPaneWidth);
            Splitter.ManipulationCompleted += SplitterOnManipulationCompleted;
        }

        public DeviceViewModel ViewModel => (DeviceViewModel)DataContext;

        public static readonly DependencyProperty IsPaneShownProperty = DependencyProperty.Register(
            "IsPaneShown", typeof(bool), typeof(DeviceView), new PropertyMetadata(default(bool), IsPaneShownChanged));
        
        public bool IsPaneShown
        {
            get => (bool)GetValue(IsPaneShownProperty);
            set => SetValue(IsPaneShownProperty, value);
        }

        private static void IsPaneShownChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var view = (DeviceView)dependencyObject;
            if (view.IsPaneShown)
            {
                view.ShowPane();
            }
            else
            {
                view.HidePane();
            }
        }

        private void ClosePane_OnClick(object sender, RoutedEventArgs e)
        {
            IsPaneShown = false;
            CharacteristicsListView.SelectedIndex = -1;
        }

        private void CharacteristicsListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IsPaneShown = CharacteristicsListView.SelectedIndex != -1;
        }

        private void SplitterOnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs manipulationCompletedRoutedEventArgs)
        {
            _applicationState.CharacteristicPaneWidth = (int)PaneColumnDefinition.Width.Value;
        }
        
        private void ShowPane()
        {
            PaneColumnDefinition.MaxWidth = Double.PositiveInfinity;
            PaneColumnDefinition.MinWidth = 400;
            Splitter.Visibility = Visibility.Visible;
        }

        private void HidePane()
        {
            Splitter.Visibility = Visibility.Collapsed;
            PaneColumnDefinition.MinWidth = 0;
            PaneColumnDefinition.MaxWidth = 0;
        }
    }
}
