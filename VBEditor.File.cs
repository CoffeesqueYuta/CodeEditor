using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        private Encoding currentEncoding = Encoding.UTF8;

        // ========== NEW FILE ==========

        private void NewFile()
        {
            editor.Clear();
            currentEncoding = Encoding.UTF8;
            currentFile = null;
            previousText = "";
            Text = "VB Editor - New File";
            SetFont(0, editor.TextLength);
            UpdateLineNumbers();
        }

        // ========== LOAD FILE AFTER DOUBLE CLICK ==========

        private void LoadFile(string path)
        {
            string contents = ReadFileAutoEncoding(path);
            editor.Text = contents;
            currentFile = path;
            previousText = editor.Text;
            Text = "VB Editor - " + Path.GetFileName(currentFile);
            SetFont(0, editor.TextLength);
            UpdateLineNumbers();
            HighlightEntireDocument();
        }

        // ========== OPEN FILE FROM MENU ==========

        private void OpenFile()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            RefreshFileExtensions(dialog);
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                string contents = ReadFileAutoEncoding(dialog.FileName);
                highlighting = true;
                editor.Text = contents;
                highlighting = false;
                currentFile = dialog.FileName;
                previousText = editor.Text;
                Text = "VB Editor - " + Path.GetFileName(currentFile);
                editor.Select(0, 0);
                SetFont(0, editor.TextLength);
                UpdateLineNumbers();
                HighlightEntireDocument();
            }
            catch (Exception ex)
            {
                highlighting = false;
                MessageBox.Show(ex.Message, "Open Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== REFRESH FILE EXTENSIONS ==========

        private void RefreshFileExtensions(FileDialog dialog)
        {
            string filters = "";
            filters += string.Join("|", syntaxHighlighter.FileExtensions.Values);
            filters += "|All Files (*.*)|*.*";
            dialog.Filter = filters;
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
                File.WriteAllText(currentFile, editor.Text, currentEncoding);
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
            RefreshFileExtensions(dialog);
            dialog.DefaultExt = "vb";
            if (dialog.ShowDialog() != DialogResult.OK) return;
            try
            {
                File.WriteAllText(dialog.FileName, editor.Text, currentEncoding);
                currentFile = dialog.FileName;
                Text = "VB Editor - " + Path.GetFileName(currentFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ReadFileAutoEncoding(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);

            // UTF-8 BOM
            if (bytes.Length >= 3 &&
                bytes[0] == 0xEF &&
                bytes[1] == 0xBB &&
                bytes[2] == 0xBF)
            {
                currentEncoding = Encoding.UTF8;
                return Encoding.UTF8.GetString(bytes);
            }

            // No BOM → Assume Shift-JIS
            currentEncoding = Encoding.GetEncoding(932);
            return currentEncoding.GetString(bytes);
        }
    }
}