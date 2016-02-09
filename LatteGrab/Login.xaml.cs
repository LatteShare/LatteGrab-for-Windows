using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using LatteGrabCore;

namespace LatteGrab
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            if (usernameTextBox.Text != "" && passwordTextBox.Password != "")
            {
                if (LatteShareConnection.Instance.RequestAPIKey(usernameTextBox.Text, passwordTextBox.Password))
                {
                    this.Close();
                } else
                {
                    MessageBox.Show("Wrong username and password combination!");
                }
            } else
            {
                MessageBox.Show("None of the fields field can be blank!");
            }
        }

        private void signupButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://grabpaw.com/signup");
        }
    }
}
