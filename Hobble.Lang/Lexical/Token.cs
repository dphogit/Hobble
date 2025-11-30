namespace Hobble.Lang.Lexical;

public record Token(string Lexeme, TokenType Type, object? Value = null, int Line = 1)
{
    public Token(string Lexeme, TokenType Type, int Line) : this(Lexeme, Type, null, Line) { }

    private T GetValue<T>()
    {
        return Value is null ? throw new InvalidCastException("Token null value cannot be casted.") : (T)Value;
    }

    public decimal GetNumberValue() => GetValue<decimal>();
    public string GetStringValue() => GetValue<string>();
}