using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;

namespace VBEditor
{
    public static class HighlightColors
    {
        public static readonly Color NormalColor = Color.Magenta;
        public static readonly Color CommentColor = Color.Lime;
        public static readonly Color KeywordColor = Color.Cyan;
        public static readonly Color TypeNameColor = Color.DarkCyan;
        public static readonly Color IdentifierColor = Color.Silver;
        public static readonly Color ParenthesisColor = Color.Yellow;
        public static readonly Color OperatorColor = Color.White;
        public static readonly Color StringColor = Color.Coral;
        public static readonly Color BackgroundColor = Color.FromArgb(34, 34, 34);
    }

    public partial class MainForm : Form
    {
        private ZoomableRichTextBox editor;
        private ZoomableRichTextBox lineNumbers;
        private Panel editorPanel;
        private ToolStripContainer toolStripContainer;

        private Timer highlightTimer;

        private bool highlighting;
        private bool  updatingLineNumbers;

        private string previousText;
        private string currentFile = null;

        private IntfSyntaxHighlighter syntaxHighlighter = new VbSyntaxHighlighter();
        private Dictionary<IntfSyntaxHighlighter, ToolStripMenuItem> languageMenuMap;
        private Dictionary<string, string> declaredIdentifiers = new Dictionary<string, string>();

        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern IntPtr GetKeyboardLayout(uint idThread);

        // ========== CONSTRUCTOR ==========

        public MainForm(string[] args)
        {
            Text = "VB Editor";
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;

            // LINE NUMBER PANE

            lineNumbers = new ZoomableRichTextBox();
            lineNumbers.Dock = DockStyle.Left;
            lineNumbers.Width = 55;
            lineNumbers.ReadOnly = true;
            lineNumbers.TabStop = false;
            lineNumbers.HideSelection = true;
            lineNumbers.SelectionLength = 0;
            lineNumbers.SelectionStart = 0;
            lineNumbers.Cursor = Cursors.Default;
            lineNumbers.WordWrap = false;
            lineNumbers.ScrollBars = RichTextBoxScrollBars.None;
            lineNumbers.Font = new Font("Consolas", 11);
            lineNumbers.BackColor = Color.FromArgb(30, 30, 30);
            lineNumbers.ForeColor = Color.Gray;
            lineNumbers.BorderStyle = BorderStyle.None;

            // MAIN EDITOR PANEL

            editor = new ZoomableRichTextBox();
            editor.Dock = DockStyle.Fill;
            editor.Font = new Font("Consolas", 11);
            editor.AcceptsTab = true;
            editor.WordWrap = false;
            editor.BackColor = HighlightColors.BackgroundColor;
            editor.ForeColor = HighlightColors.NormalColor;
            editor.BorderStyle = BorderStyle.None;
            editor.HideSelection = false;
            editor.DetectUrls = false;

            // HIGHLIGHT TIMER

            highlightTimer = new Timer();
            highlightTimer.Interval = 80;
            highlightTimer.Tick += HighlightTimer_Tick;

            // EVENTS

            editor.TextChanged += Editor_TextChanged;
            editor.VScroll += Editor_VScroll;
            editor.Resize += Editor_Resize;
            editor.KeyDown += Editor_KeyDown;
            editor.ZoomInRequested += (s, e) => ZoomIn();
            editor.ZoomOutRequested += (s, e) => ZoomOut();
            lineNumbers.Enter += (s, e) => editor.Focus();
            lineNumbers.ZoomInRequested += (s, e) => ZoomIn();
            lineNumbers.ZoomOutRequested += (s, e) => ZoomOut();

            // TOOL STRIP CONTAINER

            toolStripContainer = new ToolStripContainer();
            toolStripContainer.Dock = DockStyle.Fill;
            toolStripContainer.BackColor = HighlightColors.BackgroundColor;
            toolStripContainer.TopToolStripPanel.BackColor = SystemColors.Control;
            Controls.Add(toolStripContainer);

            // EDITOR PANEL

            editorPanel = new Panel();
            editorPanel.Dock = DockStyle.Fill;
            editorPanel.BackColor = HighlightColors.BackgroundColor;
            editorPanel.Padding = new Padding(0, 4, 0, 0);
            editorPanel.Controls.Add(editor);
            editorPanel.Controls.Add(lineNumbers);
            toolStripContainer.ContentPanel.Controls.Add(editorPanel);

            // MENU

            CreateMenu();
            previousText = editor.Text;
            if (args != null && args.Length > 0)
            {
                string path = args[0];
                if (File.Exists(path))
                {
                    LoadFile(path);
                    currentFile = path;
                }
            }
            UpdateLineNumbers();
        }

        public MainForm() : this(null)
        {
        }


        // ========== MENU ==========

        private void CreateMenu()
        {
            MenuStrip menu = new MenuStrip();

            // FILE MENU

            ToolStripMenuItem file = new ToolStripMenuItem("File");
            ToolStripMenuItem newFile = new ToolStripMenuItem("New");
            ToolStripMenuItem open = new ToolStripMenuItem("Open");
            ToolStripMenuItem save = new ToolStripMenuItem("Save");
            ToolStripMenuItem saveAs = new ToolStripMenuItem("Save As");
            ToolStripMenuItem exit = new ToolStripMenuItem("Exit");

            newFile.Click += delegate { NewFile(); };
            open.Click += delegate { OpenFile(); };
            save.Click += delegate { SaveFile(); };
            saveAs.Click += delegate { SaveFileAs(); };
            exit.Click += delegate { Close(); };
            file.DropDownItems.Add(newFile);
            file.DropDownItems.Add(open);
            file.DropDownItems.Add(save);
            file.DropDownItems.Add(saveAs);
            file.DropDownItems.Add(new ToolStripSeparator());
            file.DropDownItems.Add(exit);

            // EDIT MENU

            ToolStripMenuItem edit = new ToolStripMenuItem("Edit");
            ToolStripMenuItem undo = new ToolStripMenuItem("Undo");
            ToolStripMenuItem redo = new ToolStripMenuItem("Redo");
            ToolStripMenuItem cut = new ToolStripMenuItem("Cut");
            ToolStripMenuItem copy = new ToolStripMenuItem("Copy");
            ToolStripMenuItem paste = new ToolStripMenuItem("Paste");
            ToolStripMenuItem selectAll = new ToolStripMenuItem("Select All");
            ToolStripMenuItem find = new ToolStripMenuItem("Find");
            undo.Click += delegate { if (editor.CanUndo) editor.Undo(); };
            redo.Click += delegate { if (editor.CanRedo) editor.Redo(); };
            cut.Click += delegate { editor.Cut(); };
            copy.Click += delegate { editor.Copy(); };
            paste.Click += delegate { editor.Paste(); };
            selectAll.Click += delegate { editor.SelectAll(); };
            find.Click += delegate { FindText(); };
            edit.DropDownItems.Add(undo);
            edit.DropDownItems.Add(redo);
            edit.DropDownItems.Add(new ToolStripSeparator());
            edit.DropDownItems.Add(cut);
            edit.DropDownItems.Add(copy);
            edit.DropDownItems.Add(paste);
            edit.DropDownItems.Add(new ToolStripSeparator());
            edit.DropDownItems.Add(selectAll);
            edit.DropDownItems.Add(find);

            ToolStripMenuItem view = new ToolStripMenuItem("View");

            ToolStripMenuItem langVB = new ToolStripMenuItem("VB");
            ToolStripMenuItem langJS = new ToolStripMenuItem("JavaScript");
            ToolStripMenuItem langPS = new ToolStripMenuItem("PowerShell");
            ToolStripMenuItem langCS = new ToolStripMenuItem("C#");
            ToolStripMenuItem langPY = new ToolStripMenuItem("Python");
            ToolStripMenuItem langJAVA = new ToolStripMenuItem("Java");
            ToolStripMenuItem langCPP = new ToolStripMenuItem("C++");
            ToolStripMenuItem langFSharp = new ToolStripMenuItem("F#");
            ToolStripMenuItem langCSS = new ToolStripMenuItem("CSS");
            ToolStripMenuItem langLatex = new ToolStripMenuItem("LaTeX");
            var vb = new VbSyntaxHighlighter();
            var js = new JsSyntaxHighlighter();
            var ps = new PsSyntaxHighlighter();
            var cs = new CsSyntaxHighlighter();
            var py = new PySyntaxHighlighter();
            var java = new JavaSyntaxHighlighter();
            var cpp = new CppSyntaxHighlighter();
            var fs = new FsSyntaxHighlighter();
            var css = new CssSyntaxHighlighter();
            var latex = new LatexSyntaxHighlighter();
            languageMenuMap = new Dictionary<IntfSyntaxHighlighter, ToolStripMenuItem>
            {
                { vb, langVB },
                { js, langJS },
                { ps, langPS },
                { cs, langCS },
                { py, langPY },
                { java, langJAVA },
                { cpp, langCPP },
                { fs, langFSharp },
                { css, langCSS },
                { latex, langLatex }
            };
            langVB.Click += delegate { SetHighlighter(vb); };
            langJS.Click += delegate { SetHighlighter(js); };
            langPS.Click += delegate { SetHighlighter(ps); };
            langCS.Click += delegate { SetHighlighter(cs); };
            langPY.Click += delegate { SetHighlighter(py); };
            langJAVA.Click += delegate { SetHighlighter(java); };
            langCPP.Click += delegate { SetHighlighter(cpp); };
            langFSharp.Click += delegate { SetHighlighter(fs); };
            langCSS.Click += delegate { SetHighlighter(css); };
            langLatex.Click += delegate { SetHighlighter(latex); };
            languageMenuMap[vb].Checked = true;
            view.DropDownItems.Add(langVB);
            view.DropDownItems.Add(langJS);
            view.DropDownItems.Add(langPS);
            view.DropDownItems.Add(langCS);
            view.DropDownItems.Add(langPY);
            view.DropDownItems.Add(langJAVA);
            view.DropDownItems.Add(langCPP);
            view.DropDownItems.Add(langFSharp);
            view.DropDownItems.Add(langCSS);
            view.DropDownItems.Add(langLatex);

            menu.Items.Add(file);
            menu.Items.Add(edit);
            menu.Items.Add(view);
            MainMenuStrip = menu;
            menu.Dock = DockStyle.None;
            toolStripContainer.TopToolStripPanel.Controls.Add(menu);
        }

        // ========== KEYWORD SHORTCUTS ==========

        private void Editor_KeyDown(object sender, KeyEventArgs e)
        {
            // CTRL + S
            if (e.Control && e.KeyCode == Keys.S)
            {  
                if (currentFile == null) { SaveFileAs(); }
                else { SaveFile(); }
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // CTRL + +
            if (e.Control && e.KeyCode == Keys.Oemplus)
            {
                ZoomIn();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // CTRL + -
            if (e.Control && e.KeyCode == Keys.OemMinus)
            {
                ZoomOut();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // CTRL + Z
            if (e.Control && e.KeyCode == Keys.Z)
            {
                if (editor.CanUndo) editor.Undo();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // CTRL + Y
            if (e.Control && e.KeyCode == Keys.Y)
            {
                if (editor.CanRedo) editor.Redo();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // BACKSPACE
            if (e.KeyCode == Keys.Back)
            {
                if (editor.SelectionLength != 0) return;
                int cursor = editor.SelectionStart;
                string text = editor.Text;
                int line = TextUtils.GetLineAtPosition(text, cursor);
                int lineStart = TextUtils.GetLineStart(text, line);
                string left = text.Substring(lineStart, cursor - lineStart);
                if (left.Length == 0 && lineStart == cursor) { return; }
                if (TextUtils.IsAllSpaces(left))
                {
                    UnindentCurrentLine();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                } 
            }

            // SHIFT + TAB
            if (e.Shift && e.KeyCode == Keys.Tab)
            {
                UnindentSelectedLines();
                e.SuppressKeyPress = true;
                e.Handled = true;
                return;
            }

            // TAB
            if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                if (editor.SelectionLength > 0)
                {
                    IndentSelectedLines();
                    return;
                }
                int pos = editor.SelectionStart;
                editor.Select(pos, 0);
                editor.SelectedText = "    ";
                editor.SelectionStart = pos + 4;
                return;
            }

            // AUTO PAIRING (parenthesis & quotes)
            if (!e.Control && !e.Alt)
            {
                char open = '\0';
                char close = '\0';

                switch (e.KeyCode)
                {
                    case Keys.D9:      
                        if (IsUSKeyboard() && e.Shift) { open = '('; close = ')'; } // (
                        if (IsJapaneseKeyboard() && e.Shift) // )
                        { if (SkipClosingCharacter(e, ')')) return; }
                        break;
                    case Keys.D0:      // )
                        if (IsUSKeyboard() && e.Shift) 
                        { if (SkipClosingCharacter(e, ')')) return; }
                        break;
                    case Keys.D8:      // (
                        if (IsJapaneseKeyboard() && e.Shift) { open = '('; close = ')'; }
                        break;
                    case Keys.OemOpenBrackets:   
                        if (e.Shift) 
                        { open = '{'; close = '}'; } // {
                        else
                        { open = '['; close = ']'; } // [
                        break;
                    case Keys.Oem5: // {
                        if (IsUSKeyboard() && e.Shift) { open = '{'; close = '}'; }
                        break;
                    case Keys.Oem6:   
                        if (e.Shift)
                        { if (SkipClosingCharacter(e, '}')) return; }
                        else
                        { if (SkipClosingCharacter(e, ']')) return; }
                        break;
                    case Keys.Oem3:    // `
                        if (IsUSKeyboard() && !e.Shift) { open = '`'; close = '`'; }
                        if (IsJapaneseKeyboard() && e.Shift) 
                        { 
                            if (SkipClosingCharacter(e, '`')) return;
                            open = '`'; close = '`';
                        } 
                        break;
                    case Keys.Oem7:    // ' or "
                        if (IsUSKeyboard())
                        {
                            if (!e.Shift) { open = '\''; close = '\''; }
                            else { open = '"'; close = '"'; }
                        }
                        break;
                    case Keys.D2:
                        if (IsJapaneseKeyboard() && e.Shift) 
                        { 
                            if (SkipClosingCharacter(e, '"')) return;
                            open = '"'; close = '"'; 
                        }
                        break;
                    case Keys.D7:
                        if (IsJapaneseKeyboard() && e.Shift) 
                        { 
                            if (syntaxHighlighter is VbSyntaxHighlighter) return;
                            if (SkipClosingCharacter(e, '\'')) return;
                            open = '\''; close = '\''; 
                        }
                        break;
                    case Keys.Oemcomma: // <
                        if (e.Shift) { open = '<'; close = '>'; }
                        break;
                    case Keys.OemPeriod: // >
                        if (e.Shift)
                        { if (SkipClosingCharacter(e, '>')) return; }
                        break;
                }
                if (open != '\0')
                {
                    editor.SelectedText = open.ToString() + close.ToString();
                    editor.SelectionStart -= 1;
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
            }
        }

        bool IsJapaneseKeyboard()
        {
            IntPtr layout = GetKeyboardLayout(0);
            int langId = (int)layout & 0xFFFF;
            return langId == 0x0411; // Japanese (Japan)
        }

        bool IsUSKeyboard()
        {
            IntPtr layout = GetKeyboardLayout(0);
            int langId = (int)layout & 0xFFFF;
            return langId == 0x0409; // English (US)
        }

        private bool SkipClosingCharacter(KeyEventArgs e,char closing)
        {
            char? next = TextUtils.GetNextChar(editor.Text, editor.SelectionStart);
            if (next == closing)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                editor.SelectionStart += 1;
                return true;
            }
            return false;
        }

        // ========== TEXT CHANGED ==========

        private void Editor_TextChanged(object sender, EventArgs e)
        {
            if (highlighting) return;
            UpdateLineNumbers();
            highlightTimer.Stop();
            highlightTimer.Start();
        }

        // ========== SCROLL SYNCHRONIZATION ==========

        private void Editor_VScroll(object sender, EventArgs e)
        {
            SyncLineNumbersScroll();
        }

        private void Editor_Resize(object sender, EventArgs e)
        {
            SyncLineNumbersScroll();
        }

        // ========== SYNTAX HIGHLIGHTING ==========

        private void SetHighlighter(IntfSyntaxHighlighter highlighter)
        {
            syntaxHighlighter = highlighter;
            foreach (var kv in languageMenuMap)
                kv.Value.Checked = false;
            languageMenuMap[highlighter].Checked = true;
            HighlightEntireDocument();
        }

        // ========== MAIN ==========

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(args));
        }
    }
}
