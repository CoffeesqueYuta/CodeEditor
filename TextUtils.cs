using System;

namespace VBEditor
{
    public static class TextUtils
    {
        
        // ========== LINE START ==========

        public static int GetLineStart(string text, int lineNumber)
        {
            if (lineNumber <= 0) return 0;
            int line = 0;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    line++;
                    if (line == lineNumber) { return i + 1; }
                }
            }
            return text.Length;
        }

        // ========== LINE END ==========

        public static int GetLineEnd(string text, int lineNumber)
        {
            int start = GetLineStart(text, lineNumber);
            int i;
            for (i = start; i < text.Length; i++)
            {
                if (text[i] == '\n') { return i; }
            }
            return text.Length;
        }
        
        // ========== GET LINE FROM POSITION ==========

        public static int GetLineFromPosition(string text, int position)
        {
            if (position < 0) position = 0;
            if (position > text.Length) position = text.Length;
            int line = 0;
            int i;
            for (i = 0; i < position; i++)
            {
                if (text[i] == '\n') { line++; }
            }
            return line;
        }

        // ========== GET LINE NUMBER AT POSITION ==========

        public static int GetLineAtPosition(string text, int position)
        {
            if (position < 0) position = 0;
            if (position > text.Length) position = text.Length;
            int line = 0;
            int i;
            for (i = 0; i < position; i++)
            {
                if (text[i] == '\n') { line++; }
            }
            return line;
        }

        // ========== EXPAND LOGICAL VB RANGE ==========

        public static void ExpandLogicalRange(string text, ref int firstLine, ref int lastLine)
        {
            int totalLines = CountLines(text);
            if (firstLine <= 0) return;
            if (lastLine >= totalLines) lastLine = totalLines - 1;
            bool changed = true;
            while (changed)
            {
                changed = false;
                if (firstLine > 0)
                {
                    string previous = GetLine(text, firstLine - 1);
                    if (IsContinuationLine(previous))
                    {
                        firstLine--;
                        changed = true;
                    }
                }
            }
            changed = true;
            while (changed)
            {
                changed = false;
                if (lastLine < totalLines - 1)
                {
                    string current = GetLine(text, lastLine);
                    if (IsContinuationLine(current))
                    {
                        lastLine++;
                        changed = true;
                    }
                }
            } 
        }

        // ========== CHECK VB CONTINUATION ==========

        public static bool IsContinuationLine(string line)
        {
            if (line == null) return false;
            string trimmed = line.TrimEnd(' ', '\t', '\r');
            if (trimmed.Length == 0) return false;
            return trimmed.EndsWith("_");
        }

        // ========== COUNT LINES ==========

        public static int CountLines(string text)
        {
            if (text.Length == 0) return 1;
            int count = 1;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n') { count++; }
            }
            return count;
        }

        // ========== GET LINE ==========

        public static string GetLine(string text, int lineNumber)
        {
            if (lineNumber < 0) return "";
            int currentLine = 0;
            int start = 0;
            int i;
            for (i = 0; i < text.Length; i++)
            {
                if (text[i] == '\n')
                {
                    if (currentLine == lineNumber) { return text.Substring(start, i - start).TrimEnd('\r'); }
                    currentLine++;
                    start = i + 1;
                }
            }
            if (currentLine == lineNumber) { return text.Substring(start).TrimEnd('\r'); }
            return "";
        }

        // ========== CHECK IF STRING IS ALL SPACES ==========
        // String.All() provided by System.Linq is unavailable the preinstalled environment only, so we implement this manually.
        public static bool IsAllSpaces(string s)
        {
            for (int i = 0; i < s.Length; i++)
                if (s[i] != ' ')
                    return false;
            return true;
        }

        public static char? GetNextChar(string text, int position)
        {
            if (position >= text.Length) return null;
            return text[position];
        }
    }
}