using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace OrganizerRefactored
{
    public interface IIO
    {
        ObservableCollection<Composition> OpenFiles(ObservableCollection<Composition> playlist);
        ObservableCollection<Composition> OpenFolder(ObservableCollection<Composition> playlist);
        ObservableCollection<Composition> ReadPlaylist(ObservableCollection<Composition> playlist);
        void WritePlaylist(ObservableCollection<Composition> playlist);
        bool SaveComposition(Composition comp);
    }
}
