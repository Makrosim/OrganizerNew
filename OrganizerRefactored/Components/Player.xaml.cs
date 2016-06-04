using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Net;

namespace OrganizerRefactored
{
    /// <summary>
    /// Логика взаимодействия для Player.xaml
    /// </summary>
    public partial class Player : Page
    {
        private bool paused = false;
        public ActionCommand cmd_Play { get; set; }
        public ActionCommand cmd_Stop { get; set; }
        public ActionCommand cmd_Pause { get; set; }
        private IPlaylist IPlaylist;
        Composition comp;

        public Player(IPlaylist IPlaylist)
        {
            InitializeComponent();
            DataContext = this;
            cmd_Play = new ActionCommand(Play) { IsExecutable = true };
            cmd_Stop = new ActionCommand(Stop) { IsExecutable = true };
            cmd_Pause = new ActionCommand(Pause) { IsExecutable = true };
            this.IPlaylist = IPlaylist;
        }

        public void Play()
        {
            if (!paused)
            {
                comp = IPlaylist.GetSelectedCompositions().First();
                var path = comp.Path;
                Uri uri = new Uri(path, UriKind.Absolute);

                if (path.StartsWith("http"))
                {
                    path = System.IO.Path.Combine(Environment.CurrentDirectory, "tmpplay.mp3");
                    var wc = new WebClient();
                    wc.DownloadFileAsync(uri, path);
                    me_Player.Source = new Uri(path, UriKind.Absolute);
                }
                else
                    me_Player.Source = uri;
            }
            me_Player.Play();
            paused = false;
        }

        public void Stop()
        {
            me_Player.Stop();
            paused = false;
        }

        public void Pause()
        {
            me_Player.Pause();
            paused = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            tbl_TimeInfo.Text = me_Player.Position.ToString("mm\\:ss") + "/" + comp.Duration.ToString();
            sld_TimeLine.Value = me_Player.Position.TotalSeconds;
        }

        private void me_Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            sld_TimeLine.Maximum = me_Player.NaturalDuration.TimeSpan.TotalSeconds;
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }

        private void sld_TimeLine_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            me_Player.Position = TimeSpan.FromSeconds(sld_TimeLine.Value);
        }
    }
}
