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
using System.Windows.Threading;

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
            if (me_Player.Source == null)
                me_Player.Source = new Uri(IPlaylist.GetSelectedCompositions().First().Path, UriKind.Absolute);
            MessageBox.Show(IPlaylist.GetSelectedCompositions().First().Path);
            if (!paused)               
                me_Player.Play();
            paused = false;
        }

        public void Stop()
        {
            me_Player.Stop();
        }

        public void Pause()
        {
            paused = true;
            me_Player.Pause();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            tbl_TimeInfo.Text = me_Player.Position.ToString("mm\\:ss") + "/" + me_Player.NaturalDuration.TimeSpan.ToString("mm\\:ss");
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
