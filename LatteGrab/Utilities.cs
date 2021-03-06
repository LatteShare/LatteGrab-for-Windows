﻿using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Microsoft.Win32;

using LatteGrabCore;
using System.Threading;
using System.Runtime.InteropServices;
using System;
using System.Windows;
using System.Windows.Media;

namespace LatteGrab
{
    class Utilities
    {
        public static void UploadBitmap(Bitmap img)
        {
            var fn = Path.GetTempFileName();

            img.Save(fn, ImageFormat.Png);

            var url = LatteShareConnection.Instance.UploadFile(fn);

            System.Windows.Forms.Clipboard.SetText(url);

            System.Diagnostics.Debug.WriteLine(url);
        }

        public static void UploadAllScreens()
        {
            var fn = Path.GetTempFileName();

            FullScreenScreenshot.CaptureAllScreensToFile(fn, ImageFormat.Png);

            var url = LatteShareConnection.Instance.UploadFile(fn);

            System.Windows.Forms.Clipboard.SetText(url);

            System.Diagnostics.Debug.WriteLine(url);
        }

        public static void UploadImage(Image img)
        {
            var fn = Path.GetTempFileName();

            img.Save(fn, ImageFormat.Png);

            var url = LatteShareConnection.Instance.UploadFile(fn);

            Thread thread = new Thread(() => System.Windows.Forms.Clipboard.SetText(url));

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
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

        public static string GetBytesReadable(long i)
        {
            long absolute_i = (i < 0 ? -i : i);

            string suffix;
            double readable;

            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            
            readable = (readable / 1024);
            
            return readable.ToString("0.### ") + suffix;
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            LOGPIXELSY = 90,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }

        public static float GetScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            int logpixelsy = GetDeviceCaps(desktop, (int)DeviceCap.LOGPIXELSY);
            float screenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            float dpiScalingFactor = (float)logpixelsy / (float)96;

            if (screenScalingFactor > 1)
                return screenScalingFactor;

            return dpiScalingFactor;
        }
    }
}
