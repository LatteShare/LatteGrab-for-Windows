using System.Windows;
using System.Windows.Controls;

using LatteGrabCore;

namespace LatteGrab
{
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
            
            openAtLoginCheckBox.IsChecked = Utilities.IsRunningAtStartup();
        }

        private void openAtLoginCheckBox_ChangedState(object sender, RoutedEventArgs e)
        {
            Utilities.RunAtStartup((bool) openAtLoginCheckBox.IsChecked);
        }

        private void tabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tab_latteshare.IsSelected)
            {
                var lsi = LatteShareConnection.Instance.GetUserInformation();

                if (lsi == null)
                    return;

                var lsInfo = (LatteShareUserInformation) lsi;

                label_Username.Content = lsInfo.username;
                label_Group.Content = lsInfo.group;

                string quota = "Unmetered";

                if (lsInfo.quota != -1)
                    quota = Utilities.GetBytesReadable(lsInfo.quota);

                label_Quota.Content = Utilities.GetBytesReadable(lsInfo.usedDiskSpace) + " / " + quota;

                label_Server.Content = LatteShareConnection.Instance.CurrentServer;
                label_APIVersion.Content = LatteShareConnection.Version;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            LatteShareConnection.Instance.LogOff();
            
            new Login().Show();

            this.Close();
        }
    }
}
