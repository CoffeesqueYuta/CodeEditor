using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class LatexSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== LaTeX COMMENTS ==========

        public bool IsLineCommentPrefix(string word)
        {
            return lineCommentPrefixes.Contains(word);
        }

        private readonly HashSet<string> lineCommentPrefixes = new HashSet<string>
        {
            "%"
        };

        public bool IsBlockCommentPrefix(string word)
        {
            // LaTeX にブロックコメントは存在しない
            return false;
        }

        public bool IsBlockCommentSuffix(string word)
        {
            return false;
        }

        private readonly Dictionary<string, string> blockCommentPairs =
            new Dictionary<string, string>();

        // ========== LaTeX KEYWORDS (COMMANDS) ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "\\documentclass", "\\usepackage", "\\begin", "\\end",
            "\\title", "\\author", "\\date",
            "\\section", "\\subsection", "\\subsubsection",
            "\\paragraph", "\\subparagraph",
            "\\textbf", "\\textit", "\\underline",
            "\\emph",
            "\\item", "\\itemize", "\\enumerate",
            "\\tableofcontents",
            "\\include", "\\input",
            "\\label", "\\ref", "\\cite",
            "\\footnote",
            "\\maketitle"
        };

        // ========== LaTeX BUILT-IN TYPES (MATH COMMANDS) ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.Ordinal)
        {
            "\\frac", "\\sqrt", "\\sum", "\\prod",
            "\\int", "\\lim",
            "\\alpha", "\\beta", "\\gamma", "\\delta",
            "\\epsilon", "\\theta", "\\lambda", "\\mu", "\\pi", "\\sigma", "\\phi", "\\omega"
        };

        // ========== LaTeX DECLARATION KEYWORDS (ENVIRONMENTS) ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "document", "itemize", "enumerate", "tabular", "figure", "table",
            "align", "math", "equation"
        };

        // ========== LaTeX TYPE USAGE KEYWORDS (VALUES) ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "center", "left", "right",
            "bold", "italic"
        };

        // ========== LaTeX OPERATORS (SPECIAL SYMBOLS) ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            '\\', '{', '}', '$', '&', '^', '_', '#', '~'
        };

        // ========== LaTeX COMPOUND OPERATORS ==========

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "\\(", "\\)", "\\[", "\\]",
            "$$", "\\left", "\\right"
        };

        // ========== LaTeX Parenthesis ==========

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
