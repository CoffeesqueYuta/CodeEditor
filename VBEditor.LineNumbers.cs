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
            SendMessage(lineNumbers.Handle, WM_SETREDRAW, (IntPtr)0, IntPtr.Zero);

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
                SendMessage(lineNumbers.Handle, WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                lineNumbers.Invalidate(); 
            }
        }

        private bool ContainsJapanese(string line)
        {
            foreach (char c in line)
            {
                if (IsJapanese(c))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsJapanese(char c)
        {
            return (c >= 0x3040 && c <= 0x309F) || // ひらがな
                (c >= 0x30A0 && c <= 0x30FF) || // カタカナ
                (c >= 0x4E00 && c <= 0x9FFF) || // 漢字
                (c >= 0xFF66 && c <= 0xFF9D);   // 半角カナ
        }

    }
}