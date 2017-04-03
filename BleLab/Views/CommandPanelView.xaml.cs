using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace BleLab.Views
{
    public sealed partial class CommandPanelView : UserControl
    {
        public CommandPanelView()
        {
            this.InitializeComponent();
            Hide();
        }

        public void AddString(string s)
        {
            AddInline(new Run() { Text = s });
        }

        public void AddInline(Inline inline)
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(inline);
            AddParagraph(paragraph);
        }

        public void AddParagraph(Paragraph paragraph)
        {
            Viewer.Blocks.Add(paragraph);

            Scroller.UpdateLayout();
            Scroller.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        public void Clear()
        {
            Viewer.Blocks.Clear();
        }

        private void Expand_OnClick(object sender, RoutedEventArgs e)
        {
            Expand();
        }

        private void Hide_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Expand()
        {
            ExpandButton.Visibility = Visibility.Collapsed;
            HideButton.Visibility = Visibility.Visible;
            Scroller.Visibility = Visibility.Visible;
            ButtonsPanel.Visibility = Visibility.Visible;
        }

        private void Hide()
        {
            ExpandButton.Visibility = Visibility.Visible;
            HideButton.Visibility = Visibility.Collapsed;
            Scroller.Visibility = Visibility.Collapsed;
            ButtonsPanel.Visibility = Visibility.Collapsed;
        }
    }
}
