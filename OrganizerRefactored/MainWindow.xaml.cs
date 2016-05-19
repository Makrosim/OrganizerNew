using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();
            IIO Iio = new IO();
            IPlaylist Iplaylist = new Playlist(Iio, this);
            Player player = new Player(Iplaylist);
            TagEditor editor = new TagEditor(Iplaylist);

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
    }
}
