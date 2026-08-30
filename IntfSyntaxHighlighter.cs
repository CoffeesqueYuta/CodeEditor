using System;
using System.Drawing;

namespace VBEditor
{
    public interface IntfSyntaxHighlighter
    {
        bool IsLineCommentPrefix(string word);
        bool IsBlockCommentPrefix(string word);
        bool IsBlockCommentSuffix(string word);
        bool IsBuiltInType(string word);
        bool IsTypeUsageKeyword(string word);
        bool IsTypeDeclarationKeyword(string word);
        bool IsKeyword(string word);
        bool IsParenthesis(char c);
        bool IsOperator(char c);
        bool IsCompoundOperator(string word);
    }
}