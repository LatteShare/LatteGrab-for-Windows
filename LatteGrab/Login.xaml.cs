using System.Windows;

using LatteGrabCore;

namespace LatteGrab
{
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
