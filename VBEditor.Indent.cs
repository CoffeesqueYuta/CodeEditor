using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace VBEditor
{
    public partial class MainForm : Form
    {
        // ========== INDENT SELECTED LINES ==========

        private void IndentSelectedLines()
        {
            int selectionStart = editor.SelectionStart;
            int selectionLength = editor.SelectionLength;

            string text = editor.Text;
            int firstLine = TextUtils.GetLineAtPosition(text, selectionStart);
            int lastPosition = selectionStart + selectionLength;
            int lastLine;

            if (lastPosition > 0 && lastPosition <= text.Length && lastPosition == text.Length)
            {
                lastLine = TextUtils.GetLineAtPosition(text, text.Length - 1);
            }
            else
            {
                lastLine = TextUtils.GetLineAtPosition(text, lastPosition);
                if (lastLine > 0 && lastPosition < text.Length && text[lastPosition - 1] != '\n') { lastLine--; }
            }
            if (lastLine < firstLine) { lastLine = firstLine; }
            int blockStart = TextUtils.GetLineStart(text, firstLine);
            int blockEnd;
            if (lastLine < TextUtils.CountLines(text) - 1)
            {
                blockEnd = TextUtils.GetLineStart(text, lastLine + 1);
            }
            else
            {
                blockEnd = text.Length;
            }
            List<string> lines = new List<string>();
            int line;
            for (line = firstLine; line <= lastLine; line++)
            {
                lines.Add(TextUtils.GetLine(text, line));
            }
            string replacement = "";
            for (line = 0; line < lines.Count; line++)
            {
                replacement += "\t" + lines[line];
                if (line < lines.Count - 1) { replacement += Environment.NewLine; }
            }
            editor.Select(blockStart, blockEnd - blockStart);
            editor.SelectedText = replacement;
            int lineCont = lastLine - firstLine + 1;
            int newSelectionStart = selectionStart + 1;
            int newSelectionLength = selectionLength + lineCont;
            if (newSelectionStart > editor.TextLength) { newSelectionStart = editor.TextLength; }
            if (newSelectionStart + newSelectionLength > editor.TextLength) { newSelectionLength = editor.TextLength - newSelectionStart; }
            editor.Select(newSelectionStart, newSelectionLength);
        }

        // ========== UNINDENT SELECTED LINES ==========

        private void UnindentSelectedLines()
        {
            int selectionStart = editor.SelectionStart;
            int selectionLength = editor.SelectionLength;

            string text = editor.Text;
            int firstLine = TextUtils.GetLineAtPosition(text, selectionStart);
            int lastPosition = selectionStart + selectionLength;
            int lastLine;
            if (lastPosition > 0 && lastPosition == text.Length)
            {
                lastLine = TextUtils.GetLineAtPosition(text, text.Length - 1);
            }
            else
            {
                lastLine = TextUtils.GetLineAtPosition(text, lastPosition);
                if (lastLine > 0 && lastPosition < text.Length && text[lastPosition - 1] != '\n') { lastLine--; }
            }
            if (lastLine < firstLine) { lastLine = firstLine; }
            int blockStart = TextUtils.GetLineStart(text, firstLine);
            int blockEnd;
            if (lastLine < TextUtils.CountLines(text) - 1)
            {
                blockEnd = TextUtils.GetLineStart(text, lastLine + 1);
            }
            else
            {
                blockEnd = text.Length;
            }
            List<string> lines = new List<string>();
            int line;
            for (line = firstLine; line <= lastLine; line++)
            {
                lines.Add(TextUtils.GetLine(text, line));
            }
            string replacement = "";
            for (line = 0; line < lines.Count; line++)
            {
                replacement += "\t" + lines[line];
                if (line < lines.Count - 1) { replacement += Environment.NewLine; }
            }
            editor.Select(blockStart, blockEnd - blockStart);
            editor.SelectedText = replacement;
            int lineCount = lastLine - firstLine + 1;
            int newSelectionStart = selectionStart + 1;
            int newSelectionLength = selectionLength + lineCount;
            if (newSelectionLength > editor.TextLength) { newSelectionStart = editor.TextLength; }
            if (newSelectionStart + newSelectionLength > editor.TextLength) { newSelectionLength = editor.TextLength - newSelectionStart; }
            editor.Select(newSelectionStart, newSelectionLength);
        }

        // ========== UNINDENT CURRENT LINE ==========

        private void UnindentCurrentLine()
        {
            int cursor = editor.SelectionStart;
            string text = editor.Text;
            int line = TextUtils.GetLineAtPosition(text, cursor);
            int lineStart = TextUtils.GetLineStart(text, line);
            int lineEnd = TextUtils.GetLineEnd(text, line);
            string currentLine = text.Substring(lineStart, lineEnd - lineStart);
            int removeCount = GetIndentCharacters(currentLine);
            if (removeCount <= 0) return;
            editor.Select(lineStart, removeCount);
            editor.SelectedText = "";
            int newCursor = cursor - Math.Min(removeCount, cursor - lineStart);
            if (newCursor < lineStart) { newCursor = lineStart; }
            editor.Select(newCursor, 0);
        }

        // ========== GET INDENT CHARACTERS ==========

        private int GetIndentCharacters(string line)
        {
            if (String.IsNullOrEmpty(line)) return 0;
            if (line[0] == '\t') return 1;
            int spaces = 0;
            while (spaces < line.Length && spaces < 4 && line[spaces] == ' ') { spaces++; }
            return spaces;
        }

    }
}
