using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace LatteGrab
{
    public partial class ScreenshotHandlerForm : Form
    {
        public enum Type { FullScreen, Selection };

        private Type t;

        private GlobalHotkey ghk;

        public ScreenshotHandlerForm(Type type)
        {
            InitializeComponent();

            t = type;

            if (t == Type.FullScreen)
                ghk = new GlobalHotkey(Constants.CTRL + Constants.SHIFT, Keys.D3, this);
            else if (t == Type.Selection)
                ghk = new GlobalHotkey(Constants.CTRL + Constants.SHIFT, Keys.D4, this);
            else
                System.Diagnostics.Debug.WriteLine("Invalid type!");
            
            if (!ghk.Register())
                MessageBox.Show("Failed to register an hotkey!");
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                if (t == Type.FullScreen)
                    Utilities.UploadImage(FullScreenScreenshot.CaptureScreen());
                else if (t == Type.Selection)
                {
                    if (!ScreenshotWindow.IsCurrentlyShowing())
                        new ScreenshotWindow().Show();
                }
            }

            base.WndProc(ref m);
        }
    }
}
