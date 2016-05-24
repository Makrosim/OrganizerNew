using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class Playlist : Page, IPlaylist
    {
        public ObservableCollection<Composition> CompositionList { get; set; }
        public ActionCommand cmd_Open { get; set; }
        public ActionCommand cmd_OpenFolder { get; set; }
        public ActionCommand cmd_Remove { get; set; }
        public event EventHandler SelectionChangedEvent;
        public IIO IIO;
        public IMainWindow IMainWindow;

        public Playlist(IIO IIO, IMainWindow IMainWindow)
        {
            InitializeComponent();
            DataContext = this;
            cmd_Open = new ActionCommand(OpenFiles) { IsExecutable = true };
            cmd_OpenFolder = new ActionCommand(OpenFolder) { IsExecutable = true };
            cmd_Remove = new ActionCommand(RemoveFiles) { IsExecutable = true };
            CompositionList = new ObservableCollection<Composition>();
            this.IIO = IIO;
            this.IMainWindow = IMainWindow;
            IMainWindow.WindowLoaded += WindowLoaded;
            IMainWindow.WindowClosed += WindowClosed;
        }

        public void WindowLoaded(object sender, EventArgs e)
        {
            CompositionList = IIO.ReadPlaylist(CompositionList);
        }

        public void WindowClosed(object sender, EventArgs e)
        {
            IIO.WritePlaylist(CompositionList);
        }

        public void OpenFiles()
        {
            CompositionList = IIO.OpenFiles(CompositionList);
        }

        public void OpenFolder()
        {
            CompositionList = IIO.OpenFolder(CompositionList);
        }

        public void RemoveFiles()
        {
            List<Composition> removed = new List<Composition>();

            foreach (Composition comp in lb_List.SelectedItems)
            {
                removed.Add(comp);
            }

            foreach (Composition comp in removed)
            {
                CompositionList.Remove(comp);
            }

            RefreshCollection();
        }

        public void RefreshCollection()
        {
            foreach (Composition _comp in CompositionList)
                _comp.Number = (UInt32)CompositionList.IndexOf(_comp);
            lb_List.Items.Refresh();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChangedEvent != null)
                SelectionChangedEvent(this, e);
        }

        public List<Composition> GetSelectedComposition()
        {
            List<Composition> selected = new List<Composition>();
            if (IsMultiplySelection())
            {
                foreach (Composition comp in lb_List.SelectedItems)
                    selected.Add(comp);
                return selected;
            }
            else
            {
                selected.Add(lb_List.SelectedItem as Composition);
                return selected;
            }
        }

        private bool IsMultiplySelection()
        {
            if (lb_List.SelectedItems.Count > 1)
                return true;
            else
                return false;
        }
    }
}
