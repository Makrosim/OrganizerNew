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

namespace OrganizerRefactored
{
    public partial class ImageViewer : Page
    {
        IPlaylist Iplaylist;
        Composition comp;
        public ImageViewer(IPlaylist Iplay)
        {
            InitializeComponent();
            Iplaylist = Iplay;
            Iplaylist.SelectionChangedEvent += GetImage;
        }

        private void GetImage(object sender, EventArgs e)
        {
            if (Iplaylist.GetSelectedCompositions().Count() > 0)
            {
                comp = Iplaylist.GetSelectedCompositions().First();
                if (comp.Image != null)
                    im_Art.Source = comp.Image;
                else
                    im_Art.Source = null;
            }
        }

    }
}
