using System;
using System.Collections.Generic;

namespace VBEditor
{
    public class VbSyntaxHighlighter : IntfSyntaxHighlighter
    {
        // ========== VB READ COMMENT ==========

        public bool IsLineCommentPrefix(string word)
        {
            return lineCommentPrefixes.Contains(word);
        }

        private readonly HashSet<string> lineCommentPrefixes = new HashSet<string>
        {
            "'", "REM"
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
        };

        // ========== VB KEYWORDS ==========

        public bool IsKeyword(string word)
        {
            return keywords.Contains(word);
        }

        private readonly HashSet<string> keywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "AddHandler", "AddressOf", "Alias", "And", "AndAlso", "As", 
            "ByRef", "ByVal",
            "Call", "Case", "Catch", "Class", "Const", "Continue", 
            "Declare", "Default", "Delegate", "Dim", "Do", 
            "Each", "Else", "ElseIf", "End", "Enum", "Erase", "Error", "Event", "Exit", 
            "False", "Finally", "For", "Friend", "Function",
            "Get", "GetType", 
            "Global", "GoSub", "GoTo",
            "Handles", 
            "If", "Implements", "Imports", "In", "Inherits", "Interface", "Is", "IsNot",
            "Let", "Lib", "Like", "Long", "Loop",
            "Me", "Mod", "Module", "MustInherit", "MustOverride", "MyBase", "MyClass",
            "Namespace", "New", "Next", "Not", "Nothing", "NotInheritable", "NotOverridable",
            "Of", "On", "Operator", "Option", "Optional", "Or", "OrElse", "Overloads", "Overridable", "Overrides",
            "ParamArray", "Partial", "Private", "Property", "Protected", "Public",
            "RaiseEvent", "ReadOnly", "ReDim", "REM", "RemoveHandler", "Resume", "Return",
            "Select", "Set", "Shadows", "Shared", "Static", "Step", "Stop", "Sub", "SyncLock",
            "Then", "Throw", "To", "True", "Try", "TryCast", "TypeOf", 
            "Using", 
            "While", "With", "WithEvents", "WriteOnly",
            "Xor",
            "Yield"
        };

        // ========== VB BUILT-IN TYPE NAMES ==========

        public bool IsBuiltInType(string word)
        {
            return typeNames.Contains(word);
        }

        private readonly HashSet<string> typeNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Boolean", "Byte", "SByte", "Short", "UShort", 
            "Integer", "UInteger", "Long", "ULong", "Decimal", "Single", "Double", 
            "String", "Char",
            "Date", "Object", "Variant"
        };

        // ========== VB DECLARATION KEYWORDS ==========

        public bool IsTypeDeclarationKeyword(string word)
        {
            return typeDeclarationKeywords.Contains(word);
        }

        private readonly HashSet<string> typeDeclarationKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Class", "Module", "Structure", "Interface", "Enum", "Delegate"
        };

        // ========== VB TYPE USAGE KEYWORDS ==========

        public bool IsTypeUsageKeyword(string word)
        {
            return typeUsageKeywords.Contains(word);
        }

        private readonly HashSet<string> typeUsageKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "As", "New"
        };

        // ========== VB OPERATORS ==========

        public bool IsOperator(char c)
        {
            return operators.Contains(c);
        }

        private readonly HashSet<char> operators = new HashSet<char>
        {
            '+', '-', '*', '/', '=', '<', '>', '&', '^', '%',
        };

        public bool IsCompoundOperator(string word)
        {
            return compoundOperators.Contains(word);
        }

        private readonly HashSet<string> compoundOperators = new HashSet<string>
        {
            "<=", ">=", "<>", "+=", "-=", "*=", "/=", "&=", "^="
        };

        // ========== VB Parenthesis ==========

        public bool IsParenthesis(char c)
        {
            return parentheses.Contains(c);
        }

        private readonly HashSet<char> parentheses = new HashSet<char>
        {
            '(', ')', '[', ']'
        };


    }
}