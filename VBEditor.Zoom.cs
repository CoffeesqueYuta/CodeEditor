using System;
using System.Text.RegularExpressions;
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

                RemoveFontTags(this.SelectionStart, this.SelectionLength);
            }
            
            base.WndProc(ref m);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                e.SuppressKeyPress = true;
                HandlePaste();
                return;
            }

            base.OnKeyDown(e);
        }

        private void HandlePaste()
        {
            string text = Clipboard.GetText(TextDataFormat.UnicodeText);

            if (!string.IsNullOrEmpty(text))
            {
                int cursor = this.SelectionStart;

                // ★ Insert text manually without using RichTextBox's paste functionality
                string before = this.Text.Substring(0, cursor);
                string after  = this.Text.Substring(cursor);

                this.Text = before + text + after;

                int actualLength = this.Text.Length - cursor;

                for (int i = cursor; i < cursor + actualLength; i++)
                {
                    char c = this.Text[i];

                    if (IsJapanese(c))
                    {
                        this.Select(i, 1);
                        this.SelectionFont = new Font("Cica", this.Font.Size - 2.0f);
                    }
                    else
                    {
                        this.Select(i, 1);
                        this.SelectionFont = new Font("Consolas", this.Font.Size);
                    }
                }
                this.SelectionStart = cursor + actualLength;
            }
        }

        private bool IsJapanese(char c)
        {
            return (c >= 0x3040 && c <= 0x309F) ||      // ひらがな
                (c >= 0x30A0 && c <= 0x30FF) ||         // カタカナ
                (c >= 0x4E00 && c <= 0x9FFF) ||         // 漢字
                (c >= 0xFF66 && c <= 0xFF9D);           // 半角カナ
        }

        private void RemoveFontTags(int start, int length)
        {
            if (length <= 0) return;

            this.Select(start, length);

            string rtf = this.SelectedRtf;
            MessageBox.Show(rtf);

            rtf = Regex.Replace(rtf, @"\\f\d+", "");
            rtf = Regex.Replace(rtf, @"\\fs\d+", "");
            rtf = Regex.Replace(rtf, @"\\lang\d+", "");
            rtf = Regex.Replace(rtf, @"\\langfe\d+", "");
            rtf = Regex.Replace(rtf, @"\\langnp\d+", "");
            rtf = Regex.Replace(rtf, @"\\langfenp\d+", "");

            rtf = Regex.Replace(rtf, @"\\cf\d+", "");         // Text color
            rtf = Regex.Replace(rtf, @"\\highlight\d+", "");  // Background color

            rtf = Regex.Replace(rtf, @"\\loch", "");
            rtf = Regex.Replace(rtf, @"\\hich", "");
            rtf = Regex.Replace(rtf, @"\\dbch", "");          // ★ Japanese font specification

            rtf = Regex.Replace(rtf, @"\\b", "");             // Bold
            rtf = Regex.Replace(rtf, @"\\i", "");             // Italic

            // Delete the entire font table
            rtf = Regex.Replace(rtf, @"\{\\fonttbl.*?\}", "", RegexOptions.Singleline);

            this.SelectedRtf = rtf;

            this.Select(start + length, 0);
        }
    }
}
