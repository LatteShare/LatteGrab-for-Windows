using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

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
    }
}
