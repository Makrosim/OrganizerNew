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
        ulong appId = 5482105;
        string login = "+380952591396";
        string password = "123456qAz32q";

        public event EventHandler WindowLoaded;
        public event EventHandler WindowClosed;
        public ActionCommand cmd_Change { get; set; }

        Player player;
        TagEditor editor;
        IPlaylist Iplaylist;
        IIO Iio;
        IIO Iovk;
        ImageViewer img;
        Diagram diag;
        IAutohorize auth;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            App.LanguageChanged += LanguageChanged;
            cmd_Change = new ActionCommand(ChangeLanguage) { IsExecutable = true };

            Iio = new IO();
            Iplaylist = new Playlist(Iio, this);
            player = new Player(Iplaylist);
            editor = new TagEditor(Iplaylist);

            fr_TagEdiror.NavigationService.Navigate(editor);
            fr_Player.NavigationService.Navigate(player);
            fr_Playlist.NavigationService.Navigate(Iplaylist);
            rbtn_EditLocal.IsChecked = true;
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

        private void btn_EditVK_Checked(object sender, RoutedEventArgs e)
        {
            auth = new AuthForm();
            auth.SignIn += SignedIn;
            fr_Playlist.NavigationService.Navigate(auth);
        }

        private void SignedIn(object sender, EventArgs e)
        {
            string[] AuthParams = auth.GetParams();
            Iovk = new IOVK(appId, AuthParams[0], AuthParams[1]);
            diag = new Diagram(Iplaylist, Iovk);
            fr_Diagram.NavigationService.Navigate(diag);

            try
            {
                Iplaylist.SwitchToVK(Iovk);
            }
            catch
            {
                MessageBox.Show("AuthError");
                Iplaylist.SwitchToLocal(Iio);
            }
            fr_Playlist.NavigationService.Navigate(Iplaylist);
        }

        private void btn_EditLocal_Checked(object sender, RoutedEventArgs e)
        {
            Iplaylist.SwitchToLocal(Iio);
            img = new ImageViewer(Iplaylist);
            fr_Diagram.NavigationService.Navigate(img);
        }

    }
}
