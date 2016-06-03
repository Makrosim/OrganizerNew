using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrganizerRefactored
{
    interface IAutohorize
    {
        string[] GetParams();
        event EventHandler SignIn;
    }
}
