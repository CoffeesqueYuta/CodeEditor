using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class PySyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== PYTHON FILE EXTENSIONS ==========

        public bool IsFileExtension(string extension)
        {
            return fileExtensions.ContainsValue(extension);
        }

        public Dictionary<string, string> FileExtensions
        {
            get { return fileExtensions; }
        }

        private readonly Dictionary<string, string> fileExtensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".py", "Python Files (*.py)|*.py" }
        };

        // ========== PYTHON COMMENTS ==========

        public bool IsLineCommentPrefix(string word)
        {
            return lineCommentPrefixes.Contains(word);
        }

        private readonly HashSet<string> lineCommentPrefixes = new HashSet<string>
        {
            "#"
        };

        public bool IsBlockCommentPrefix(string word)
        {
            return blockCommentPairs.ContainsKey(word);
        }

        public bool IsBlockCommentSuffix(string word)
        {
            return blockCommentPairs.ContainsValue(word);
        }

        // Python は公式のブロックコメント構文を持たないが、
        // """ と ''' を「コメント扱い」するエディタが多いので辞書化する
        private readonly Dictionary<string, string> blockCommentPairs =
            new Dictionary<string, string>
        {
            { "\"\"\"", "\"\"\"" },
            { "'''", "'''" }
        };

        // ========== PYTHON KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "False", "None", "True",
            "and", "as", "assert",
            "break",
            "class", "continue",
            "def", "del",
            "elif", "else",
            "except",
            "finally", "for", "from",
            "global",
            "if", "import", "in", "is",
            "lambda",
            "nonlocal", "not",
            "or",
            "pass",
            "raise", "return",
            "try",
            "while", "with",
            "yield"
        };

        // ========== PYTHON BUILT-IN TYPES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "int", "float", "bool", "str", "bytes",
            "list", "tuple", "dict", "set", "frozenset",
            "complex",
            "object"
        };

        // ========== PYTHON DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "class"
        };

        // ========== PYTHON TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            // Python は new が無いので空
        };

        // ========== PYTHON OPERATORS ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            '+', '-', '*', '/', '%',
            '=', '<', '>', '!',
            '&', '|', '^', '~',
            ':', ',', '.', '@'
        };

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "==", "!=", "<=", ">=",
            "+=", "-=", "*=", "/=", "%=",
            "//", "//=", "**", "**=",
            "->"
        };

        // ========== PYTHON Parenthesis ==========

        public bool IsParenthesis(char c)
        {
            return parentheses.Contains(c);
        }

        private readonly HashSet<char> parentheses = new HashSet<char>
        {
            '(', ')', '{', '}', '[', ']'
        };
    }
}
