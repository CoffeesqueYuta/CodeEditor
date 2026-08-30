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

        private const int WM_SETREDRAW = 0x000B;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        

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
                    LoadFile(path);   // ← あなたのファイル読み込み処理
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
            // ToolStripMenuItem langBash = new ToolStripMenuItem("Bash");
            ToolStripMenuItem langLatex = new ToolStripMenuItem("LaTeX");
            // ToolStripMenuItem langC = new ToolStripMenuItem("C");
            langVB.Click += delegate { SetHighlighter(new VbSyntaxHighlighter()); };
            langJS.Click += delegate { SetHighlighter(new JsSyntaxHighlighter()); };
            langPS.Click += delegate { SetHighlighter(new PsSyntaxHighlighter()); };
            langCS.Click += delegate { SetHighlighter(new CsSyntaxHighlighter()); };
            langPY.Click += delegate { SetHighlighter(new PySyntaxHighlighter()); };
            langJAVA.Click += delegate { SetHighlighter(new JavaSyntaxHighlighter()); };
            langCPP.Click += delegate { SetHighlighter(new CppSyntaxHighlighter()); };
            langFSharp.Click += delegate { SetHighlighter(new FsSyntaxHighlighter()); };
            langCSS.Click += delegate { SetHighlighter(new CssSyntaxHighlighter()); };
            // langBash.Click += delegate { SetHighlighter(new BashSyntaxHighlighter()); };
            langLatex.Click += delegate { SetHighlighter(new LatexSyntaxHighlighter()); };
            // langC.Click += delegate { SetHighlighter(new CSyntaxHighlighter()); };

            view.DropDownItems.Add(langVB);
            view.DropDownItems.Add(langJS);
            view.DropDownItems.Add(langPS);
            view.DropDownItems.Add(langCS);
            view.DropDownItems.Add(langPY);
            view.DropDownItems.Add(langJAVA);
            view.DropDownItems.Add(langCPP);
            view.DropDownItems.Add(langFSharp);
            view.DropDownItems.Add(langCSS);
            // view.DropDownItems.Add(langBash);
            view.DropDownItems.Add(langLatex);
            // view.DropDownItems.Add(langC);

            menu.Items.Add(file);
            menu.Items.Add(edit);
            menu.Items.Add(view);
            MainMenuStrip = menu;
            menu.Dock = DockStyle.None;
            toolStripContainer.TopToolStripPanel.Controls.Add(menu);
        }

        private void LoadFile(string path)
        {
            editor.Text = File.ReadAllText(path);
            currentFile = path;
            previousText = editor.Text;

            UpdateLineNumbers();
            HighlightEntireDocument();
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

            // TAB
            if (e.KeyCode == Keys.Tab)
            {
                if (editor.SelectionLength > 0)
                {
                    if (e.Shift)
                    {
                        UnindentSelectedLines();
                    }
                    else
                    {
                        IndentSelectedLines();
                    }
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
                if (e.Shift)
                {
                    UnindentSelectedLines();
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                    return;
                }
            }
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

        private IntfSyntaxHighlighter syntaxHighlighter = new VbSyntaxHighlighter();

        private void SetHighlighter(IntfSyntaxHighlighter highlighter)
        {
            syntaxHighlighter = highlighter;
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
