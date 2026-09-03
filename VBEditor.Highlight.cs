using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            int msg,
            IntPtr wParam,
            ref CHARFORMAT2 lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct CHARFORMAT2
        {
            public int cbSize;
            public uint dwMask;
            public uint dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szFaceName;
            public ushort wWeight;
            public ushort sSpacing;
            public int crBackColor;
            public int lcId;
            public uint dwReserved;
            public short sStyle;
            public short wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;
            public byte bReserved1;
        }

        const int EM_SETCHARFORMAT = 0x0444;
        const int SCF_SELECTION = 0x0001;
        const uint CFM_COLOR = 0x40000000;
        const uint CFM_BACKCOLOR = 0x04000000;

        // ========== INITIAL FULL HIGHLIGHT ==========

        private void HighlightEntireDocument()
        {
            if (editor.TextLength == 0) return;
            HighlightLines(editor.Text, 0, TextUtils.CountLines(editor.Text) - 1);
        }

        // ========== HIGHLIGHT TIMER ==========

        private void HighlightTimer_Tick(object sender, EventArgs e)
        {
            highlightTimer.Stop();
            if (!highlighting) HighlightChangedRegion();
        }

        // ========== FIND CHANGED REGION ==========

        private void HighlightChangedRegion()
        {
            string newText = editor.Text;
            string oldText = previousText;
            int prefix = 0;
            int minimum = Math.Min(oldText.Length, newText.Length);
            while (prefix < minimum && oldText[prefix] == newText[prefix]) { prefix++; }
            int oldSuffix = oldText.Length - 1;
            int newSuffix = newText.Length - 1;
            while (oldSuffix >= prefix && newSuffix >= prefix && oldText[oldSuffix] == newText[newSuffix])
            {
                oldSuffix--;
                newSuffix--;
            }
            int start = prefix;
            int end = newSuffix + 1;
            if (start > newText.Length) { start = newText.Length; }
            if (end > newText.Length) { end = newText.Length; }
            int startLine = TextUtils.GetLineAtPosition(newText, start);
            int endLine = TextUtils.GetLineAtPosition(newText, end);
            int oldStartLine = TextUtils.GetLineAtPosition(oldText, Math.Min(prefix, oldText.Length));
            int oldEndLine = TextUtils.GetLineAtPosition(oldText, Math.Min(oldSuffix + 1, oldText.Length));
            int firstLine = startLine;
            int lastLine = endLine;
            if (oldStartLine < firstLine) { firstLine = oldStartLine; }
            if (oldEndLine > lastLine) { lastLine = oldEndLine; }
            TextUtils.ExpandLogicalRange(newText, ref firstLine, ref lastLine);
            if (newText.Length == 0) { previousText = newText; return;}
            HighlightLines(newText, firstLine, lastLine);
            previousText = newText;
        }

        // ========== HIGHLIGHT SELECTED LINES ==========

        private void HighlightLines(string text, int firstLine, int lastLine)
        {
            int start = TextUtils.GetLineStart(text, firstLine);
            int end = TextUtils.GetLineEnd(text, lastLine);
            if (start < 0) start = 0;
            if (end < start) return;
            if (end > editor.TextLength) end = editor.TextLength;
            int cursor = editor.SelectionStart;
            int selectionLength = editor.SelectionLength;
            highlighting = true;
            try {
                SendMessage(editor.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
                editor.SuspendLayout();
                SetColor(start, end - start, HighlightColors.NormalColor, HighlightColors.BackgroundColor);
                HighlightRange(
                    text,
                    start,
                    end,
                    (s, l, fore, back) => SetColor(s, l, fore, back)
                );

            }
            finally 
            {
                if (cursor > editor.TextLength) { cursor = editor.TextLength; }
                if (selectionLength > editor.TextLength - cursor) { selectionLength = editor.TextLength - cursor; }
                editor.Select(cursor, selectionLength);
                editor.ResumeLayout();
                SendMessage(editor.Handle, WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
                editor.Invalidate();
                editor.Update();
                highlighting = false;
            }
        }

        private void HighlightRange(string text, int start, int end, Action<int, int, Color, Color> SetColor)
        {
            int i = start;
            while (i < end)
            {
                char c = text[i];

                // COMMENT

                int commentLen;
                if (TryReadComment(text, i, end, out commentLen))
                {
                    SetColor(i, commentLen, HighlightColors.CommentColor, HighlightColors.BackgroundColor);
                    i += commentLen;
                    continue;
                }

                // STRING

                if (c == '"')
                {
                    int stringStart = i;
                    i++;
                    while (i < end)
                    {
                        if (text[i] == '"')
                        {
                            if (i + 1 < end && text[i + 1] == '"') { i += 2; continue; }
                            i++; break;
                        }
                        i++;
                    }
                    SetColor(stringStart, i - stringStart, HighlightColors.StringColor, HighlightColors.BackgroundColor);
                    continue;
                }

                // IDENTIFIER / KEYWORD / TYPE
                string original;
                if (Char.IsLetter(c) || c == '_')
                {
                    int wordStart = i;
                    i++;
                    while (i < end && (Char.IsLetterOrDigit(text[i]) || text[i] == '_')) { i++; }
                    string word = text.Substring(wordStart, i - wordStart);

                    // BUILT-IN TYPE
                    if (syntaxHighlighter.IsBuiltInType(word))
                    { SetColor(wordStart, i - wordStart, HighlightColors.TypeNameColor, HighlightColors.BackgroundColor); }

                    // USER TYPE AFTER AS / NEW
                    else if (IsTypeNameAfterKeyword(text, wordStart))
                    { SetColor(wordStart, i - wordStart, HighlightColors.TypeNameColor, HighlightColors.BackgroundColor); }

                    // USER TYPE AFTER CLASS / STRUCTURE / INTERFACE / ENUM / DELEGATE
                    else if (IsTypeNameAfterDeclarationKeyword(text, wordStart))
                    { 
                        SetColor(wordStart, i - wordStart, HighlightColors.TypeNameColor, HighlightColors.BackgroundColor); 
                        string lower = word.ToLower();
                        if (!declaredIdentifiers.ContainsKey(lower))
                        {
                            declaredIdentifiers[lower] = word;
                        }
                    }

                    // KEYWORD
                    else if (syntaxHighlighter.IsKeyword(word))
                    { SetColor(wordStart, i - wordStart, HighlightColors.KeywordColor, HighlightColors.BackgroundColor); }

                    else if (declaredIdentifiers.TryGetValue(word.ToLower(), out original))
                    {
                        int wordLength = i - wordStart;

                        editor.Select(wordStart, wordLength);
                        editor.SelectedText = original;
                        editor.SelectionStart = wordStart + original.Length;
                    }

                    // NORMAL IDENTIFIER
                    else
                    { SetColor(wordStart, i - wordStart, HighlightColors.IdentifierColor, HighlightColors.BackgroundColor); }
                    continue;
                }

                // NUMBER

                if (Char.IsDigit(c))
                {
                    i++;
                    while (i < end && (Char.IsDigit(text[i]) || text[i] == '.' || Char.ToLower(text[i]) == 'e')) { i++; }
                    continue;
                }

                // PARENTHESIS / BRACKETS

                int parenLen;
                if (TryReadParenthesis(text, i, end, out parenLen))
                {
                    SetColor(i, parenLen, HighlightColors.ParenthesisColor, HighlightColors.BackgroundColor);
                    i += parenLen;
                    continue;
                }

                // OPERATORS

                int opLen;
                if (TryReadOperator(text, i, end, out opLen))
                {
                    SetColor(i, opLen, HighlightColors.OperatorColor, HighlightColors.BackgroundColor);
                    i += opLen;
                    continue;
                }
                i++;
            }
        }

        private string NextWord(string text, int index, int end)
        {
            int i = index;

            // 空白を飛ばす
            while (i < end && Char.IsWhiteSpace(text[i])) i++;

            int start = i;

            // 単語を読む
            while (i < end && (Char.IsLetterOrDigit(text[i]) || text[i] == '_')) i++;

            if (i > start)
                return text.Substring(start, i - start);

            return null;
        }
        
        private bool TryReadComment(string text, int index, int endIndex, out int length)
        {
            length = 0;
            // LINE COMMENT
            for (int prefixLen = 1; prefixLen <= 4; prefixLen++)
            {
                if (index + prefixLen > endIndex)
                    break;
                string candidate = text.Substring(index, prefixLen);
                if (syntaxHighlighter.IsLineCommentPrefix(candidate))
                {
                    int i = index + prefixLen;
                    while (i < endIndex && text[i] != '\n')
                        i++;
                    length = i - index;
                    return true;
                }
            }

            // BLOCK COMMENT
            for (int prefixLen = 1; prefixLen <= 4; prefixLen++)
            {
                if (index + prefixLen > endIndex)
                    break;
                string candidatePrefix = text.Substring(index, prefixLen);
                if (syntaxHighlighter.IsBlockCommentPrefix(candidatePrefix))
                {
                    int i = index + prefixLen;
                    for (int suffixLen = 1; suffixLen <= 8; suffixLen++)
                    {
                        while (i + suffixLen <= endIndex)
                        {
                            string candidateSuffix = text.Substring(i, suffixLen);
                            if (syntaxHighlighter.IsBlockCommentSuffix(candidateSuffix))
                            {
                                i += suffixLen;
                                length = i - index;
                                return true;
                            }
                            i++;
                        }
                    }
                    length = endIndex - index;
                    return true;
                }
            }
            return false;
        }

        private bool TryReadParenthesis(string text, int index, int endIndex, out int length)
        {
            length = 0;
            char c = text[index];
            if (!syntaxHighlighter.IsParenthesis(c))
                return false;
            length = 1;
            return true;
        }

        private bool TryReadOperator(string text, int startIndex, int endIndex, out int length)
        {
            length = 0;
            char c = text[startIndex];
            if (!syntaxHighlighter.IsOperator(c))
                return false;
            int i = startIndex + 1;
            if (i < endIndex)
            {
                char next = text[i];
                if (syntaxHighlighter.IsCompoundOperator(c.ToString() + next.ToString())) { i++; }
            }
            length = i - startIndex;
            return true;
        }
        
        public bool IsTypeNameAfterKeyword(string text, int wordStart)
        {
            int position = wordStart - 1;
            while (position >= 0 && Char.IsWhiteSpace(text[position])) { position--; }
            if (position < 0) return false;
            int end = position + 1;
            while (position >= 0 && (Char.IsLetterOrDigit(text[position]) || text[position] == '_')) { position--; }
            int start = position + 1;
            if (start >= end) return false;
            string previousWord = text.Substring(start, end - start);
            return syntaxHighlighter.IsTypeUsageKeyword(previousWord);
        }

        public bool IsTypeNameAfterDeclarationKeyword(string text, int wordStart)
        {
            int position = wordStart - 1;
            while (position >= 0 && Char.IsWhiteSpace(text[position])) { position--; }
            if (position < 0) return false;
            int end = position + 1;
            while (position >= 0 && (Char.IsLetterOrDigit(text[position]) || text[position] == '_')) { position--; }
            int start = position + 1;
            if (start >= end) return false;
            string previousWord = text.Substring(start, end - start);
            return syntaxHighlighter.IsTypeDeclarationKeyword(previousWord);
        }

        private void SetColor(int start, int length, Color fore, Color back)
        {
            if (length <= 0) return;

            editor.Select(start, length);

            CHARFORMAT2 cf = new CHARFORMAT2();
            cf.cbSize = Marshal.SizeOf(cf);
            cf.dwMask = CFM_COLOR | CFM_BACKCOLOR;
            cf.crTextColor = ColorTranslator.ToWin32(fore);
            cf.crBackColor = ColorTranslator.ToWin32(back);

            SendMessage(editor.Handle, EM_SETCHARFORMAT, (IntPtr)SCF_SELECTION, ref cf);
        }
    }
}