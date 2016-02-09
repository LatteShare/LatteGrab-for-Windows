using System.Runtime.InteropServices;
using System.Windows.Forms;

using SharpShell.Attributes;
using SharpShell.SharpContextMenu;

using LatteGrabCore;

namespace LatteGrabFileUploadExtension
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.AllFiles)]

    public class UploadFileExtension : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var itemUpload = new ToolStripMenuItem("Upload using LatteGrab");

            itemUpload.Click += (sender, args) => UploadFile();
            menu.Items.Add(itemUpload);

            return menu;
        }

        private void UploadFile()
        {
            foreach (var filePath in SelectedItemPaths)
            {
                Clipboard.SetText(LatteShareConnection.Instance.UploadFile(filePath));
            }
        }
    }
}
