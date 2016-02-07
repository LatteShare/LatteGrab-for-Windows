using System.Windows;

namespace LatteGrab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            new ScreenshotHandlerForm(ScreenshotHandlerForm.Type.Selection).Show();
            new ScreenshotHandlerForm(ScreenshotHandlerForm.Type.FullScreen).Show();

            if (!LatteShareConnection.Instance.CheckAPIKey())
                new Login().Show();
            else
                System.Diagnostics.Debug.WriteLine("Successfully logged in.");

            LatteShareConnection.Instance.SetDelegates(new UploadSuccessful(ShowUploadedBalloon), new UploadError(ShowErrorBalloon));
        }

        private void ShowUploadedBalloon(string url)
        {
            string title = "LatteGrab - Upload Successful!";
            string text = "The URL to your file is now on your clipboard!";
            
            taskbarIcon.ShowBalloonTip(title, text, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info);
        }

        public void ShowErrorBalloon(string error)
        {
            string title = "LatteGrab - Upload Error!";
            string text = error;
            
            taskbarIcon.ShowBalloonTip(title, text, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Error);
        }

        private void MenuItem_MyFiles_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://latte.edr.io/files/");
        }

        private void MenuItem_LogOff_Click(object sender, RoutedEventArgs e)
        {
            LatteShareConnection.Instance.LogOff();

            new Login().Show();
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
