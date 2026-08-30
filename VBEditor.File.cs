using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        // ========== NEW FILE ==========

        private void NewFile()
        {
            editor.Clear();
            currentFile = null;
            previousText = "";
            Text = "VB Editor - New File";
            UpdateLineNumbers();
        }

        // ========== OPEN FILE ==========

        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "VB Files (*.vb)|*.vb|Class Files (*.cls)|*.cls|All Files (*.*)|*.*";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                string contents = File.ReadAllText(dialog.FileName);
                highlighting = true;
                editor.Text = contents;
                highlighting = false;
                currentFile = dialog.FileName;
                //previousText = editor.contents;
                previousText = editor.Text;
                Text = "VB Editor - " + Path.GetFileName(currentFile);
                editor.Select(0, 0);
                UpdateLineNumbers();
                HighlightEntireDocument();
            }
            catch (Exception ex)
            {
                highlighting = false;
                MessageBox.Show(ex.Message, "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== SAVE FILE ==========

        private void SaveFile()
        {
            if (currentFile == null)
            {
                SaveFileAs();
                return;
            }
            try
            {
                File.WriteAllText(currentFile, editor.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== SAVE FILE AS ==========

        private void SaveFileAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "VB Files (*.vb)|*.vb|Class Files (*.cls)|*.cls|All Files (*.*)|*.*";
            dialog.DefaultExt = "vb";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                File.WriteAllText(dialog.FileName, editor.Text);
                currentFile = dialog.FileName;
                Text = "VB Editor - " + Path.GetFileName(currentFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}