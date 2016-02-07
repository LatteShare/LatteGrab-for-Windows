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
        }
    }
}
