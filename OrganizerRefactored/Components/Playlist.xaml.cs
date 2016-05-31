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
        public event EventHandler CollectionFilled;
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
            CollectionFilled?.Invoke(this, new EventArgs());
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
                _comp.Number = (UInt32)CompositionList.IndexOf(_comp) + 1;
            lb_List.Items.Refresh();
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedEvent?.Invoke(this, e);
        }

        public List<Composition> GetSelectedCompositions()
        {
            List<Composition> selected = new List<Composition>();
            if (lb_List.SelectedItems.Count != 0)
            {
                if (lb_List.SelectedItems.Count > 1)
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
            else
                return selected;
        }

        public void SwitchToVK(IIO IoVK)
        {
            IIO.WritePlaylist(CompositionList);
            lb_List.SelectedItems.Clear();
            CompositionList.Clear();
            IIO = IoVK;
            CompositionList = IIO.ReadPlaylist(CompositionList);
            CollectionFilled?.Invoke(this, new EventArgs());
            btn_Open.IsEnabled = false;
            btn_OpenFolder.IsEnabled = false;
        }

        public void SwitchToLocal(IIO IO)
        {
            CompositionList.Clear();
            IIO = IO;
            CompositionList = IIO.ReadPlaylist(CompositionList);
            CollectionFilled?.Invoke(this, new EventArgs());
            btn_Open.IsEnabled = true;
            btn_OpenFolder.IsEnabled = true;
        }

        public bool SaveComposition(Composition comp)
        {
            return IIO.SaveComposition(comp);
        }

        public List<Composition> GetAllCompositions()
        {
            return CompositionList.ToList<Composition>();
        }

    }
}
