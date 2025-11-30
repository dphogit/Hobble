namespace Hobble.Lang.Lexical;

public class TokenFactory(int line = 1)
{
    private int _line = line;

    public Token Number(string lexeme, decimal value) => Create(lexeme, TokenType.Number, value);
    public Token String(string value) => Create($"\"{value}\"", TokenType.String, value);

    public Token Plus() => Create("+", TokenType.Plus);
    public Token Minus() => Create("-", TokenType.Minus);
    public Token Star() => Create("*", TokenType.Star);
    public Token Slash() => Create("/", TokenType.Slash);
    public Token LeftParen() => Create("(", TokenType.LeftParen);
    public Token RightParen() => Create(")", TokenType.RightParen);
    public Token Eof() => Create("", TokenType.Eof);

    public Token Error(string message) => Create(message, TokenType.Error);

    private Token Create(string lexeme, TokenType type) => new(lexeme, type, _line);
    private Token Create(string lexeme, TokenType type, object? value)  => new(lexeme, type, value, _line);

    public void SetLine(int line) => _line = line;
    public void NextLine() => _line++;

    public Token FromChar(char c)
    {
        return c switch
        {
            '+' => Plus(),
            '-' => Minus(),
            '*' => Star(),
            '/' => Slash(),
            _ => throw new ArgumentException($"Unknown token to create from '{c}'")
        };
    }
}