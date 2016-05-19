using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Fingerprinter;

namespace OrganizerRefactored
{
    /// <summary>
    /// Логика взаимодействия для TagEditor.xaml
    /// </summary>
    public partial class TagEditor : Page, INotifyPropertyChanged
    {
        public ActionCommand cmd_Save { get; set; }
        public ActionCommand cmd_Recognize { get; set; }
        public Composition OldComposition { get; set; }
        public Composition Composition { get; set; }
        public bool IsMultiselect { get; set; }

        private List<Composition> CompList;
        private IPlaylist ICompList;
        private bool Saved = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public TagEditor(IPlaylist complist)
        {
            InitializeComponent();
            DataContext = this;
            cmd_Save = new ActionCommand(SaveTags) { IsExecutable = true };
            cmd_Recognize = new ActionCommand(RecognizeComposition) { IsExecutable = true };
            ICompList = complist;
            ICompList.SelectionChangedEvent += SelectionChanged;
        }

        protected void OnPropertyChanged(string property)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void SelectionChanged(object sender, EventArgs e)
        {
            CompList = ICompList.GetSelectedComposition();

            if (CompList != null)
                Composition = CompList.First();        

            if (CompList.Count > 1)
                IsMultiselect = true;
            else
                IsMultiselect = false;
            OldComposition = null;
            Saved = true;
            OnPropertyChanged("Composition");
            OnPropertyChanged("IsMultiselect");

        }

        public void SaveTags()
        {
            if (IsMultiselect)
            {
                foreach (Composition comp in CompList)
                {
                    if (chb_Performers.IsChecked == true)
                        comp.Performers = Composition.Performers;
                    if (chb_Album.IsChecked == true)
                        comp.Album = Composition.Album;
                    if (chb_Genres.IsChecked == true)
                        comp.Genres = Composition.Genres;
                    if (chb_Year.IsChecked == true)
                        comp.Year = Composition.Year;
                    comp.SaveChanges(); // копировать
                }
            }
            else
            {
                Composition.SaveChanges();
            }
            ICompList.RefreshCollection();
        }

        public void RecognizeComposition()
        {
            var Recognizer = new Recognizer();
            string[] tags = Recognizer.Recognize(Composition.Path);

            if (tags[0].Equals("Error"))
                MessageBox.Show(tags[1] + ": " + tags[2], tags[0]);
            else
            {
                tbx_Title.Text = tags[0];
                tbx_Performers.Text = tags[1];
                tbx_ID.Text = tags[2];
            }
        }

        private void btn_Previous_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!Saved)
            {
                var tmp = new Composition();
                tmp = Composition.Clone(tmp); // tmp = редактир аудиозапись
                Composition = OldComposition.Clone(Composition); // текущая = нередактир
                OldComposition = tmp.Clone(OldComposition); // старая = редактир
                OnPropertyChanged("Composition");
            }
        }

        private void btn_Previous_MouseLeave(object sender, MouseEventArgs e)
        {
            if(!Saved)
            { 
                var tmp = new Composition();
                tmp = Composition.Clone(tmp);
                Composition = OldComposition.Clone(Composition);
                OldComposition = tmp.Clone(OldComposition);
                OnPropertyChanged("Composition");
            }
        }

        private void TagEdited(object sender, KeyEventArgs e)
        {
            if (Saved)
            {
                OldComposition = new Composition();
                OldComposition = Composition.Clone(OldComposition);
                Saved = false;
            }
        }

        private void btn_Previous_Click(object sender, RoutedEventArgs e)
        {
            OldComposition = null;
            Saved = true;
        }
    }
}
