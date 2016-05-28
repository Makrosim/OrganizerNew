using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizerRefactored
{
    public interface IPlaylist
    {
        event EventHandler SelectionChangedEvent;
        List<Composition> GetSelectedComposition();
        void RefreshCollection();
        void SaveComposition(Composition comp);
    }
}
