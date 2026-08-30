using System;
using System.Drawing;
using System.Windows.Forms;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        
        // ========== FIND ==========

        private void FindText()
        {
            Form findForm = new Form();
            findForm.Text = "Find";
            findForm.Width = 400;
            findForm.Height = 110;
            findForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            findForm.StartPosition = FormStartPosition.CenterParent;
            TextBox search = new TextBox();
            search.Left = 10;
            search.Top = 10;
            search.Width = 280;
            Button findButton = new Button();
            findButton.Text = "Find";
            findButton.Left = 300;
            findButton.Top = 8;
            findButton.Width = 70;
            findButton.Click += delegate
            {
                string value = search.Text;
                if (String.IsNullOrEmpty(value)) return;
                int start = editor.SelectionStart + editor.SelectionLength;
                int position = editor.Text.IndexOf(value, start, StringComparison.OrdinalIgnoreCase);
                if (position < 0)
                {
                    position = editor.Text.IndexOf(value, 0, StringComparison.OrdinalIgnoreCase);
                }
                if (position >= 0)
                {
                    editor.Select(position, value.Length);
                    editor.Focus();
                }
                else
                {
                    MessageBox.Show("Text not found.");
                }
            };
            findForm.Controls.Add(search);
            findForm.Controls.Add(findButton);
            findForm.AcceptButton = findButton;
            findForm.Show(this);
        }
    }
}