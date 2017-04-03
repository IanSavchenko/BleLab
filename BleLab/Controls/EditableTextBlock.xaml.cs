using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BleLab.Controls
{
    public sealed partial class EditableTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(EditableTextBlock), new PropertyMetadata(default(string), TextPropertyChangedCallback));

        public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
            "TextWrapping", typeof(TextWrapping), typeof(EditableTextBlock), new PropertyMetadata(default(TextWrapping)));

        public static readonly DependencyProperty TextBlockStyleProperty = DependencyProperty.Register(
            "TextBlockStyle", typeof(Style), typeof(EditableTextBlock), new PropertyMetadata(default(Style)));

        public Style TextBlockStyle
        {
            get { return (Style)GetValue(TextBlockStyleProperty); }
            set { SetValue(TextBlockStyleProperty, value); }
        }
        
        private CoreCursor _oldCursor;

        public EditableTextBlock()
        {
            this.InitializeComponent();
            SwitchToNonEditingMode();
        }

        public TextWrapping TextWrapping
        {
            get { return (TextWrapping)GetValue(TextWrappingProperty); }
            set { SetValue(TextWrappingProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private void SwitchToNonEditingMode()
        {
            Editable.Visibility = Visibility.Collapsed;
            NonEditable.Visibility = Visibility.Visible;
        }

        private void SwitchToEditingMode()
        {
            Editable.Visibility = Visibility.Visible;
            NonEditable.Visibility = Visibility.Collapsed;

            Editable.Focus(FocusState.Programmatic);
            Editable.SelectAll();
        }

        private void NonEditable_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            SwitchToEditingMode();
        }

        private void Editable_OnLostFocus(object sender, RoutedEventArgs e)
        {
            SwitchToNonEditingMode();
            Text = Editable.Text;
        }

        private static void TextPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var target = (EditableTextBlock)dependencyObject;

            target.Editable.Text = (string)dependencyPropertyChangedEventArgs.NewValue ?? "";
            target.NonEditable.Text = (string)dependencyPropertyChangedEventArgs.NewValue ?? "";
        }
        
        private void NonEditable_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _oldCursor = Window.Current.CoreWindow.PointerCursor;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.IBeam, 0);
        }

        private void NonEditable_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = _oldCursor;
        }

        private void Editable_OnKeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
                SwitchToNonEditingMode();
        }
    }
}
