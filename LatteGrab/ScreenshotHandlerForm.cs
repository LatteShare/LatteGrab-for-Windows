using System.Windows.Forms;

namespace LatteGrab
{
    public partial class ScreenshotHandlerForm : Form
    {
        public enum Type { FullScreen, Selection };

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;

                Params.ExStyle |= 0x80;

                return Params;
            }
        }

        private Type t;

        private GlobalHotkey ghk;
        private GlobalHotkey escGhk;

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

            escGhk = new GlobalHotkey(Constants.NOMOD, Keys.Escape, this);
            
            if (!ghk.Register())
                MessageBox.Show("Failed to register an hotkey!");

            this.Visible = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                if (t == Type.FullScreen)
                    Utilities.UploadAllScreens();
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
