using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class CssSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== CSS COMMENTS ==========

        public bool IsLineCommentPrefix(string word)
        {
            // CSS に行コメントは存在しない
            return false;
        }

        private readonly HashSet<string> lineCommentPrefixes = new HashSet<string>();

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

        // ========== CSS KEYWORDS (AT-RULES) ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "@import", "@media", "@supports", "@font-face", "@keyframes",
            "@namespace", "@page", "@charset", "@counter-style"
        };

        // ========== CSS BUILT-IN TYPES (DATA TYPES) ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "color", "length", "percentage", "number", "integer",
            "angle", "time", "resolution", "url", "string"
        };

        // ========== CSS DECLARATION KEYWORDS (SELECTORS) ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "class", "id", "element", "pseudo", "attribute"
        };

        // ========== CSS TYPE USAGE KEYWORDS (VALUES) ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "auto", "inherit", "initial", "unset", "revert"
        };

        // ========== CSS OPERATORS ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            ':', ';', ',', '.', '#', '.', '*', '>', '+', '~', '='
        };

        // ========== CSS COMPOUND OPERATORS ==========

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "::", ":=", "!="
        };

        // ========== CSS Parenthesis ==========

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
