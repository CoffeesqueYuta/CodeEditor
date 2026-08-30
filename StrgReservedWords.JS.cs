using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class JsSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== JS COMMENTS ==========

        public bool IsLineCommentPrefix(string word)
        {
            return lineCommentPrefixes.Contains(word);
        }

        private readonly HashSet<string> lineCommentPrefixes = new HashSet<string>
        {
            "//"
        };

        public bool IsBlockCommentPrefix(string word)
        {
            return blockCommentPairs.ContainsKey(word);
        }

        public bool IsBlockCommentSuffix(string word)
        {
            return blockCommentPairs.ContainsValue(word);
        }

        private readonly Dictionary<string, string> blockCommentPairs =
            new Dictionary<string, string>
        {
            { "/*", "*/" }
        };

        // ========== JS KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "break", "case", "catch", "class", "const", "continue",
            "debugger", "default", "delete", "do",
            "else", "enum", "export", "extends",
            "false", "finally", "for", "function",
            "if", "import", "in", "instanceof",
            "let",
            "new", "null",
            "return",
            "super", "switch",
            "this", "throw", "true", "try", "typeof",
            "var", "void",
            "while", "with",
            "yield"
        };

        // ========== JS BUILT-IN TYPES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "string", "number", "boolean", "bigint", "symbol", "object", "undefined",
            "Array", "Map", "Set", "WeakMap", "WeakSet",
            "Date", "RegExp", "Promise", "Function"
        };

        // ========== JS DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "class"
        };

        // ========== JS TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "new"
        };

        // ========== JS OPERATORS ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            '+', '-', '*', '/', '%',
            '=', '<', '>', '!',
            '&', '|', '^', '~',
            '?', ':'
        };

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "==", "!=", "===", "!==",
            "<=", ">=",
            "&&", "||",
            "+=", "-=", "*=", "/=", "%=",
            "<<", ">>", ">>>",
            "<<=", ">>=", ">>>=",
            "&=", "|=", "^="
        };

        // ========== JS Parenthesis ==========

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
