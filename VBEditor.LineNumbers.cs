using System;
using System.Drawing;
using System.Windows.Forms;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        // ========== LINE NUMBERS ==========

        private void UpdateLineNumbers()
        {
            if (updatingLineNumbers) return;
            updatingLineNumbers = true;
            try
            {
                int lineCount = editor.GetLineFromCharIndex(editor.TextLength) + 1;
                int digits = lineCount.ToString().Length;
                string numbers = "";
                int i;
                for (i = 1; i <= lineCount; i++)
                {
                    numbers += i.ToString().PadLeft(digits) + Environment.NewLine;
                }
                lineNumbers.Text = numbers;
                SyncLineNumbersScroll();
            }
            finally
            {
                updatingLineNumbers = false;
            }
        }

    }
}