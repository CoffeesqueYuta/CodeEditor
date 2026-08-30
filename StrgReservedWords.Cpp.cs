using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class CppSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== C++ COMMENTS ==========

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

        // ========== C++ KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "alignas", "alignof", "and", "and_eq", "asm", "auto",
            "bitand", "bitor", "bool", "break",
            "case", "catch", "char", "char16_t", "char32_t", "class", "compl", "const",
            "constexpr", "const_cast", "continue",
            "decltype", "default", "delete", "do", "double", "dynamic_cast",
            "else", "enum", "explicit", "export", "extern",
            "false", "float", "for", "friend",
            "goto",
            "if", "inline", "int",
            "long",
            "mutable",
            "namespace", "new", "noexcept", "not", "not_eq", "nullptr",
            "operator", "or", "or_eq",
            "private", "protected", "public",
            "register", "reinterpret_cast", "return",
            "short", "signed", "sizeof", "static", "static_assert", "static_cast", "struct", "switch",
            "template", "this", "thread_local", "throw", "true", "try", "typedef", "typeid", "typename",
            "union", "unsigned", "using",
            "virtual", "void", "volatile",
            "wchar_t", "while",
            "xor", "xor_eq"
        };

        // ========== C++ BUILT-IN TYPES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "bool", "char", "wchar_t", "char16_t", "char32_t",
            "short", "int", "long",
            "signed", "unsigned",
            "float", "double",
            "void",
            "size_t", "ptrdiff_t"
        };

        // ========== C++ DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "class", "struct", "union", "enum"
        };

        // ========== C++ TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "new", "delete", "sizeof", "typeid"
        };

        // ========== C++ OPERATORS ==========

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
            "&&", "||",
            "+=", "-=", "*=", "/=", "%=",
            "<<", ">>",
            "<<=", ">>=",
            "&=", "|=", "^=",
            "->", "::"
        };

        // ========== C++ Parenthesis ==========

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
