using System.Diagnostics.CodeAnalysis;

namespace Hobble.Lang.Lexical;

public static class Keywords
{
    private static readonly Dictionary<string, TokenType> TokenTypes = new()
    {
        { "false", TokenType.False },
        { "true", TokenType.True },
        { "print", TokenType.Print },
        { "var", TokenType.Var },
        { "if", TokenType.If },
        { "else", TokenType.Else }
    };

    private static readonly HashSet<TokenType> StatementStarters =
    [
        TokenType.If,
        TokenType.Print,
        TokenType.Var
    ];

    public static bool TryGetTokenType(string keyword, [NotNullWhen(true)] out TokenType? tokenType)
    {
        if (TokenTypes.TryGetValue(keyword, out var type))
        {
            tokenType = type;
            return true;
        }
        
        tokenType = null;
        return false;
    }

    public static bool IsStatementStarter(TokenType type)
    {
        return StatementStarters.Contains(type);
    }
}