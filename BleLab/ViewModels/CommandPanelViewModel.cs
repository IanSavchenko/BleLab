using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using BleLab.Views;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class CommandPanelViewModel : Screen
    {
        private readonly CoreDispatcher _dispatcher;
        private CommandPanelView _view;
        private int _fontSize;

        public CommandPanelViewModel()
        {
            _dispatcher = Window.Current.Dispatcher;
            FontSize = 10;
        }

        public void AddMessage(object message)
        {
            var ignored = _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
                {
                    var paragraph = message as Paragraph;
                    if (paragraph != null)
                    {
                        _view.AddParagraph(paragraph);
                        return;
                    }

                    var inline = message as Inline;
                    if (inline != null)
                    {
                        _view.AddInline(inline);
                        return;
                    }

                    _view.AddString(message.ToString());
                });
        }

        public int FontSize
        {
            get { return _fontSize; }
            set
            {
                if (value == _fontSize) return;
                _fontSize = value;
                NotifyOfPropertyChange();
            }
        }

        public bool CanIncFontSize => FontSize < 18;

        public bool CanDecFontSize => FontSize > 8;

        public void IncFontSize()
        {
            FontSize++;
            NotifyOfPropertyChange(nameof(CanIncFontSize));
            NotifyOfPropertyChange(nameof(CanDecFontSize));
        }

        public void DecFontSize()
        {
            FontSize--;
            NotifyOfPropertyChange(nameof(CanIncFontSize));
            NotifyOfPropertyChange(nameof(CanDecFontSize));
        }

        public void ClearMessages()
        {
            _view.Clear();
        }

        protected override void OnViewAttached(object view, object context)
        {
            _view = (CommandPanelView)view;
        }

        protected override void OnViewLoaded(object view)
        {

        }
    }
}
