using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

//  Based on http://www.codeproject.com/Articles/91487/Screen-Capture-in-WPF-WinForms-Application?msg=4737859

namespace LatteGrab
{
    public partial class ScreenshotWindow : Window
    {
        public double x;
        public double y;
        public double width;
        public double height;
        public bool isMouseDown = false;

        private static bool isCurrentlyShowing = false;

        public static Boolean IsCurrentlyShowing()
        {
            return isCurrentlyShowing;
        }

        public ScreenshotWindow()
        {
            InitializeComponent();

            ScreenshotWindow.isCurrentlyShowing = true;

            this.Cursor = Cursors.Cross;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = true;

            x = e.GetPosition(null).X;
            y = e.GetPosition(null).Y;
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (x == e.GetPosition(null).X && y == e.GetPosition(null).Y)
            {
                cnv.Children.Clear();

                this.x = this.y = 0;
                this.isMouseDown = false;

                ScreenshotWindow.isCurrentlyShowing = false;

                this.Cursor = Cursors.Arrow;

                this.Close();
            }
        }

        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.isMouseDown)
            {
                double curx = e.GetPosition(null).X;
                double cury = e.GetPosition(null).Y;

                System.Windows.Shapes.Rectangle r = new System.Windows.Shapes.Rectangle();
                SolidColorBrush brush = new SolidColorBrush(Colors.White);

                r.Stroke = brush;
                r.Fill = brush;
                r.StrokeThickness = 1;
                r.Width = Math.Abs(curx - x);
                r.Height = Math.Abs(cury - y);

                cnv.Children.Clear();
                cnv.Children.Add(r);

                Canvas.SetLeft(r, Math.Min(x, curx));
                Canvas.SetTop(r, Math.Min(y, cury));

                if (e.LeftButton == MouseButtonState.Released)
                {
                    cnv.Children.Clear();

                    width = Math.Abs(e.GetPosition(null).X - x);
                    height = Math.Abs(e.GetPosition(null).Y - y);

                    this.CaptureScreen(Math.Min(x, curx), Math.Min(y, cury), width, height);

                    this.x = this.y = 0;
                    this.isMouseDown = false;

                    ScreenshotWindow.isCurrentlyShowing = false;

                    this.Cursor = Cursors.Arrow;

                    this.Close();
                }
            }
        }

        public void CaptureScreen(double x, double y, double width, double height)
        {
            int ix, iy, iw, ih;

            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);

            try
            {
                Bitmap image = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(image);

                g.CopyFromScreen(ix - 7, iy - 7, 0, 0, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);

                Task.Run(() => Utilities.UploadImage(image));
            }
            catch
            {
                isCurrentlyShowing = false;

                this.Close();
            }
        }

        public void SaveScreen(double x, double y, double width, double height)
        {
            int ix, iy, iw, ih;

            ix = Convert.ToInt32(x);
            iy = Convert.ToInt32(y);
            iw = Convert.ToInt32(width);
            ih = Convert.ToInt32(height);

            try
            {
                Bitmap myImage = new Bitmap(iw, ih);

                Graphics gr1 = Graphics.FromImage(myImage);

                IntPtr dc1 = gr1.GetHdc();
                IntPtr dc2 = NativeMethods.GetWindowDC(NativeMethods.GetForegroundWindow());

                NativeMethods.BitBlt(dc1, ix, iy, iw, ih, dc2, ix, iy, 13369376);

                gr1.ReleaseHdc(dc1);

                Utilities.UploadImage(myImage);
            }
            catch
            {
                isCurrentlyShowing = false;
            }
        }

        internal class NativeMethods
        {

            [DllImport("user32.dll")]
            public extern static IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hwnd);
            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetForegroundWindow();
            [DllImport("gdi32.dll")]
            public static extern UInt64 BitBlt(IntPtr hDestDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, System.Int32 dwRop);

        }
    }
}
