using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class JsSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== JS FILE EXTENSIONS ==========

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
            { ".js", "JavaScript Files (*.js)|*.js" },
            { ".mjs", "JavaScript Module Files (*.mjs)|*.mjs" },
            { ".cjs", "JavaScript CommonJS Files (*.cjs)|*.cjs" }, 
            { ".jsx", "JavaScript React Files (*.jsx)|*.jsx" },
            {".json", "JSON Files (*.json)|*.json" },
            { ".jsonc", "JSON with Comments Files (*.jsonc)|*.jsonc" },
            { ".json5", "JSON5 Files (*.json5)|*.json5" },
            { ".d.ts", "TypeScript Declaration Files (*.d.ts)|*.d.ts" },
            { ".d.mts", "TypeScript Declaration Module Files (*.d.mts)|*.d.mts" },
            { ".d.cts", "TypeScript Declaration CommonJS Files (*.d.cts)|*.d.cts" },
            { ".ts", "TypeScript Files (*.ts)|*.ts" },
            { ".tsx", "TypeScript React Files (*.tsx)|*.tsx" }
        };
        
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
