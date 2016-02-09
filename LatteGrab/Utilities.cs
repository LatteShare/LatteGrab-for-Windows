using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Microsoft.Win32;

using LatteGrabCore;

namespace LatteGrab
{
    class Utilities
    {
        public static void UploadImage(Bitmap img)
        {
            var fn = Path.GetTempFileName();

            img.Save(fn, ImageFormat.Png);

            var url = LatteShareConnection.Instance.UploadFile(fn);

            System.Windows.Forms.Clipboard.SetText(url);

            System.Diagnostics.Debug.WriteLine(url);
        }

        public static void UploadImage(Image img)
        {
            var fn = Path.GetTempFileName();

            img.Save(fn, ImageFormat.Png);

            var url = LatteShareConnection.Instance.UploadFile(fn);

            System.Windows.Forms.Clipboard.SetText(url);

            System.Diagnostics.Debug.WriteLine(url);
        }

        public static void RunAtStartup(bool shouldRun)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (shouldRun)
                    key.SetValue("LatteGrab", "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\"");
                else
                    key.DeleteValue("LatteGrab", false);
            }
        }

        public static bool IsRunningAtStartup()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                return (key.GetValue("LatteGrab") != null);
            }
        }
    }
}
