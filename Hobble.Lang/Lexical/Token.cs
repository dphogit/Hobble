namespace Hobble.Lang.Lexical;

public record Token(string Lexeme, TokenType Type, object? Value = null, int Line = 1)
{
    public Token(string Lexeme, TokenType Type, int Line) : this(Lexeme, Type, null, Line) { }
}