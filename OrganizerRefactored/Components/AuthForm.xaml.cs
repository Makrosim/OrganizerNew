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
    public partial class AuthForm : Page, IAutohorize
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public event EventHandler SignIn;

        public AuthForm()
        {
            InitializeComponent();
            DataContext = this;
        }

        public string[] GetParams()
        {
            string[] authparams = new string[2];
            authparams[0] = Login;
            authparams[1] = Password;
            return authparams;
        }

        private void btn_Autohorize_Click(object sender, RoutedEventArgs e)
        {
            SignIn?.Invoke(this, e);
        }
    }
}
