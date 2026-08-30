using System;
using System.Drawing;
using System.Windows.Forms;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        private void SyncLineNumbersScroll()
        {
            if (!editor.IsHandleCreated || !lineNumbers.IsHandleCreated) return;
            int firstLine = editor.GetLineFromCharIndex(editor.GetCharIndexFromPosition(new Point(0, 0)));
            int target = lineNumbers.GetFirstCharIndexFromLine(firstLine);
            if (target >= 0)
            {
                lineNumbers.Select(target, 0);
                lineNumbers.ScrollToCaret();
            }
        }
    }
}