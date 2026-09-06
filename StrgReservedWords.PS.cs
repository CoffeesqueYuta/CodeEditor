using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class PsSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== POWERSHELL FILE EXTENSIONS ==========

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
            { ".ps1", "PowerShell Scripts (*.ps1)|*.ps1" },
            { ".psm1", "PowerShell Modules (*.psm1)|*.psm1" },
            { ".psd1", "PowerShell Data Files (*.psd1)|*.psd1" }
        };
        
        // ========== POWERSHELL COMMENTS ==========

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

        private readonly Dictionary<string, string> blockCommentPairs =
            new Dictionary<string, string>
        {
            { "<#", "#>" }
        };

        // ========== POWERSHELL KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "function", "filter", "workflow",
            "if", "else", "elseif",
            "switch",
            "for", "foreach", "while", "do", "until",
            "return", "break", "continue",
            "try", "catch", "finally", "throw",
            "class", "enum",
            "begin", "process", "end",
            "in", "param",
            "true", "false",
            "null"
        };

        // ========== POWERSHELL BUILT-IN TYPES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "string", "int", "int32", "int64",
            "bool", "boolean",
            "double", "float",
            "decimal",
            "char",
            "byte",
            "array", "hashtable",
            "datetime",
            "object"
        };

        // ========== POWERSHELL DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "class", "enum"
        };

        // ========== POWERSHELL TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "new"
        };

        // ========== POWERSHELL OPERATORS ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            '+', '-', '*', '/', '%',
            '=', '<', '>', '!',
            '&', '|', '^'
        };

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "-eq", "-ne", "-gt", "-ge", "-lt", "-le",
            "-like", "-notlike",
            "-match", "-notmatch",
            "-contains", "-notcontains",
            "-in", "-notin",
            "-replace",
            "-and", "-or", "-xor",
            "-is", "-isnot"
        };

        // ========== POWERSHELL Parenthesis ==========

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
