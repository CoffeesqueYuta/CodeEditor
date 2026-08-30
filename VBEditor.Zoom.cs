using System;
using System.Drawing;
using System.Windows.Forms;

namespace VBEditor
{
    public partial class MainForm : Form
    {

        private float zoomSize = 12f;   

        private void ApplyZoom()
        {
            editor.Font = new Font(editor.Font.FontFamily, zoomSize);
            lineNumbers.Font = new Font(lineNumbers.Font.FontFamily, zoomSize);
            HighlightEntireDocument();
        }

        private void ZoomIn()
        {
            zoomSize += 1f;
            if (zoomSize < 5f) zoomSize = 5f;
            ApplyZoom();
        }

        private void ZoomOut()
        {
            zoomSize -= 1f;
            if (zoomSize < 5f) zoomSize = 5f;
            ApplyZoom();
        }
    }

    public class ZoomableRichTextBox : RichTextBox
    {
        public event EventHandler ZoomInRequested;
        public event EventHandler ZoomOutRequested;

        private bool wheelLock = false;

        protected override void WndProc(ref Message m)
        {
            const int WM_MOUSEWHEEL = 0x020A;
            const int WM_PASTE = 0x0302;

            if (m.Msg == WM_MOUSEWHEEL)
            {
                // Handle IntPtr directly (never convert to int)
                IntPtr wparam = m.WParam;

                // Extract only the leading 16bit as short
                int delta = (short)((long)wparam >> 16);

                bool ctrl = (Control.ModifierKeys & Keys.Control) == Keys.Control;

                if (ctrl)
                {
                    if (delta > 0)
                    {
                        if (ZoomInRequested != null)
                            ZoomInRequested(this, EventArgs.Empty);
                    }
                    else if (delta < 0)
                    {
                        if (ZoomOutRequested != null)
                            ZoomOutRequested(this, EventArgs.Empty);
                    }

                    return;
                }
            }

            if (m.Msg == WM_PASTE)
            {
                string text = null;

                if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
                    text = Clipboard.GetText(TextDataFormat.UnicodeText);
                else if (Clipboard.ContainsText(TextDataFormat.Text))
                    text = Clipboard.GetText(TextDataFormat.Text);
                else if (Clipboard.ContainsText(TextDataFormat.Rtf))
                    text = Clipboard.GetText(TextDataFormat.Text);

                if (text != null)
                {
                    this.SelectedText = text;
                    return;
                }
            }

            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                //cp.ExStyle &= ~0x00000020; // WS_EX_ACCEPTFILES を無効化
                //cp.ClassName = "RICHEDIT50W";
                return cp;
            }
        }


    }


}
