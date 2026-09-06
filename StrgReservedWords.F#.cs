using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class FsSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== F# FILE EXTENSIONS ==========

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
            { ".fs", "F# Files (*.fs)|*.fs" },
            { ".fsi", "F# Signature Files (*.fsi)|*.fsi" },
            { ".fsx", "F# Script Files (*.fsx)|*.fsx" }
        };
        
        // ========== F# COMMENTS ==========

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
            { "(*", "*)" }
        };

        // ========== F# KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "and", "as", "assert",
            "base", "begin",
            "class", "default", "delegate", "do", "done", "downcast", "downto",
            "elif", "else", "end", "exception", "extern",
            "false", "finally", "for", "fun", "function",
            "global",
            "if", "in", "inherit", "inline", "interface", "internal",
            "lazy", "let",
            "match", "member", "module", "mutable",
            "namespace", "new", "null",
            "of", "open", "or", "override",
            "private", "public",
            "rec", "return",
            "select", "static",
            "then", "to", "true", "try", "type",
            "upcast", "use",
            "val", "void",
            "when", "while", "with",
            "yield"
        };

        // ========== F# BUILT-IN TYPES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "int", "int32", "int64",
            "float", "double", "decimal",
            "bool", "string", "char",
            "unit", "byte", "sbyte",
            "list", "array", "seq", "map", "set",
            "option"
        };

        // ========== F# DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "type", "class", "interface"
        };

        // ========== F# TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "new"
        };

        // ========== F# OPERATORS ==========

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
            "==", "!=", "<=", ">=",
            "+=", "-=", "*=", "/=", "%=",
            "->", "<-", "::", "|>", "<|"
        };

        // ========== F# Parenthesis ==========

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
