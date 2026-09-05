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
            int lastLine = TextUtils.GetLineAtPosition(text, lastPosition);
            if (lastLine < firstLine) { lastLine = firstLine; }
            int blockStart = TextUtils.GetLineStart(text, firstLine);
            int blockEnd = TextUtils.GetLineStart(text, lastLine) + TextUtils.GetLine(text, lastLine).Length;
            List<string> lines = new List<string>();
            for (int line = firstLine; line <= lastLine; line++)
                lines.Add(TextUtils.GetLine(text, line));

            string replacement = "";
            int totalAdded = 0;
            int firstLineAdded = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int add = GetIndentCharacters(line);
                if (i == 0) { firstLineAdded = add; }
                if (add > 0) { line = new string(' ', add) + line; }
                totalAdded += add;
                replacement += line;
                if (i < lines.Count - 1) { replacement += Environment.NewLine; }
            }
            editor.Select(blockStart, blockEnd - blockStart);
            editor.SelectedText = replacement;
            int newStart = blockStart + (selectionStart - blockStart + firstLineAdded);
            int newEnd = blockStart + (blockEnd - blockStart + totalAdded);
            int newLength = Math.Max(newEnd - newStart, 0);
            editor.Select(newStart, newLength);
        }

        // ========== UNINDENT SELECTED LINES ==========

        private void UnindentSelectedLines()
        {
            int selectionStart = editor.SelectionStart;
            int selectionLength = editor.SelectionLength;
            string text = editor.Text;
            int firstLine = TextUtils.GetLineAtPosition(text, selectionStart);
            int lastPosition = selectionStart + selectionLength;
            int lastLine = TextUtils.GetLineAtPosition(text, lastPosition);
            if (lastLine < firstLine) { lastLine = firstLine; }
            int blockStart = TextUtils.GetLineStart(text, firstLine);
            int oldEnd = TextUtils.GetLineStart(text, lastLine) + TextUtils.GetLine(text, lastLine).Length;
            int blockEnd = oldEnd;
            List<string> lines = new List<string>();
            for (int line = firstLine; line <= lastLine; line++)
                lines.Add(TextUtils.GetLine(text, line));

            string replacement = "";
            int totalRemoved = 0;
            int firstLineRemoved = 0;

            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                int remove = GetUnindentCharacters(line);
                if (i == 0) { firstLineRemoved = remove; }
                if (remove > 0) { line = line.Substring(remove);}
                totalRemoved += remove;
                replacement += line;
                if (i < lines.Count - 1) { replacement += Environment.NewLine; }
            }
            editor.Select(blockStart, blockEnd - blockStart);
            editor.SelectedText = replacement;
            int newStart = blockStart + (selectionStart - blockStart - firstLineRemoved);
            if (newStart < blockStart) {newStart = blockStart; }
            int newEnd = oldEnd - totalRemoved;
            if (newEnd > blockEnd) { newEnd = blockEnd; }
            int newLength = Math.Max(newEnd - newStart, 0);
            editor.Select(newStart, newLength);
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
            int removeCount = GetUnindentCharacters(currentLine);
            if (removeCount <= 0) return;
            editor.Select(lineStart, removeCount);
            editor.SelectedText = "";
            int newCursor = cursor - removeCount;
            if (newCursor < lineStart) { newCursor = lineStart; }
            editor.Select(newCursor, 0);
        }

        // ========== GET INDENT CHARACTERS ==========

        private int GetIndentCharacters(string line)
        {
            if (String.IsNullOrEmpty(line)) return 0;
            int spaces = 0;
            while (spaces < line.Length && line[spaces] == ' ')
                spaces++;
            int mod = spaces % 4;
            return mod == 0 ? 4 : (4 - mod);
        }

        private int GetUnindentCharacters(string line)
        {
            if (String.IsNullOrEmpty(line)) return 0;
            int spaces = 0;
            while (spaces < line.Length && line[spaces] == ' ')
                spaces++;
            if (spaces == 0)
                return 0;
            int mod = spaces % 4;
            return mod == 0 ? 4 : mod;
        }

    }
}
