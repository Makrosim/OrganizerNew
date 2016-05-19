using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizerRefactored
{
    public interface IMainWindow
    {
        event EventHandler WindowLoaded;
        event EventHandler WindowClosed;
    }
}
