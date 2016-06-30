using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

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

        /* Code to Disable WinKey, Alt+Tab, Ctrl+Esc Starts Here */

        // Structure contain information about low-level keyboard input event 
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }
        //System level functions to be used for hook and unhook keyboard input  
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);
        //Declaring Global objects     
        private IntPtr ptrHook;
        private LowLevelKeyboardProc objKeyboardProcess;

        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            if (nCode >= 0 && ScreenshotWindow.IsCurrentlyShowing())
            {
                KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

                // Disabling Windows keys 

                if (objKeyInfo.key == Keys.RWin || objKeyInfo.key == Keys.LWin || objKeyInfo.key == Keys.Tab && HasAltModifier(objKeyInfo.flags) || objKeyInfo.key == Keys.Escape && (Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    return (IntPtr) 1; // if 0 is returned then All the above keys will be enabled
                }
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        bool HasAltModifier(int flags)
        {
            return (flags & 0x20) == 0x20;
        }

        /* Code to Disable WinKey, Alt+Tab, Ctrl+Esc Ends Here */

        public ScreenshotWindow()
        {
            InitializeComponent();

            ScreenshotWindow.isCurrentlyShowing = true;

            this.Cursor = System.Windows.Input.Cursors.Cross;

            this.Topmost = true;

            this.Activate();

            this.CatchSpecialKeyCombos();
        }

        private void CatchSpecialKeyCombos()
        {
            ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
            objKeyboardProcess = new LowLevelKeyboardProc(captureKey);
            ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
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

                this.Cursor = System.Windows.Input.Cursors.Arrow;

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

                    this.Cursor = System.Windows.Input.Cursors.Arrow;

                    this.Close();
                }
            }
        }

        public void CaptureScreen(double x, double y, double width, double height)
        {
            int ix, iy, iw, ih;

            ix = Convert.ToInt32(x * Utilities.GetScalingFactor());
            iy = Convert.ToInt32(y * Utilities.GetScalingFactor());
            iw = Convert.ToInt32(width * Utilities.GetScalingFactor());
            ih = Convert.ToInt32(height * Utilities.GetScalingFactor());

            try
            {
                Bitmap image = new Bitmap(iw, ih, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(image);

                g.CopyFromScreen(ix - Convert.ToInt32(7 * Utilities.GetScalingFactor()), iy - Convert.ToInt32(7 * Utilities.GetScalingFactor()), 0, 0, new System.Drawing.Size(iw, ih), CopyPixelOperation.SourceCopy);

                Task.Run(() => Utilities.UploadImage(image));
            }
            catch
            {
                isCurrentlyShowing = false;

                this.Close();
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
