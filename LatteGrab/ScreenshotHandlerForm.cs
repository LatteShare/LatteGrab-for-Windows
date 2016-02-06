using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LatteGrab
{
    public partial class ScreenshotHandlerForm : Form
    {
        private GlobalHotkey ghk;

        public ScreenshotHandlerForm()
        {
            InitializeComponent();

            ghk = new GlobalHotkey(Constants.CTRL + Constants.SHIFT, Keys.D4, this);

            if (ghk.Register())
                WriteLine("Hotkey registered.");
            else
                WriteLine("Hotkey failed to register");
        }

        private void HandleHotkey()
        {
            WriteLine("Hotkey pressed!");

            ScreenshotWindow w = new ScreenshotWindow();

            w.Show();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
                HandleHotkey();
            base.WndProc(ref m);
        }

        private void WriteLine(string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
        }
    }
}
