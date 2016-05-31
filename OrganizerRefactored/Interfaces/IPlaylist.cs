using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizerRefactored
{
    public interface IPlaylist
    {
        event EventHandler SelectionChangedEvent;
        event EventHandler CollectionFilled;
        List<Composition> GetSelectedCompositions();
        List<Composition> GetAllCompositions();
        bool SaveComposition(Composition comp);
        void RefreshCollection();
        void SwitchToVK(IIO IoVK);
        void SwitchToLocal(IIO IO);
    }
}
