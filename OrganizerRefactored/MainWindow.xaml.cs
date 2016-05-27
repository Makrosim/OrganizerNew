using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrganizerRefactored
{
    public partial class MainWindow : Window, IMainWindow
    {
        public event EventHandler WindowLoaded;
        public event EventHandler WindowClosed;
        public ActionCommand cmd_Change { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            App.LanguageChanged += LanguageChanged;
            IIO Iio = new IO();
            IPlaylist Iplaylist = new Playlist(Iio, this);
            Player player = new Player(Iplaylist);
            TagEditor editor = new TagEditor(Iplaylist);
            cmd_Change = new ActionCommand(ChangeLanguage) { IsExecutable = true };

            fr_TagEdiror.NavigationService.Navigate(editor);
            fr_Player.NavigationService.Navigate(player);
            fr_CompList.NavigationService.Navigate(Iplaylist);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowLoaded(this, e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            WindowClosed(this, e);
        }

        private void LanguageChanged(Object sender, EventArgs e)
        {

        }

        private void ChangeLanguage()
        {
            if (App.Language.Equals(new CultureInfo("ru-RU")))
            {
                App.Language = new CultureInfo("en-US");
            }
            else
            {
                App.Language = new CultureInfo("ru-RU");
            }
        }
    }
}
