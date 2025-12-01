namespace Hobble.Lang.Lexical;

public class TokenFactory(int line = 1)
{
    private int _line = line;

    public Token Number(string lexeme, decimal value) => Create(lexeme, TokenType.Number, value);
    public Token String(string value) => Create($"\"{value}\"", TokenType.String, value);
    
    public Token True() => Create("true", TokenType.True);
    public Token False() => Create("false", TokenType.False);

    public Token Plus() => Create("+", TokenType.Plus);
    public Token Minus() => Create("-", TokenType.Minus);
    public Token Star() => Create("*", TokenType.Star);
    public Token Slash() => Create("/", TokenType.Slash);
    public Token LeftParen() => Create("(", TokenType.LeftParen);
    public Token RightParen() => Create(")", TokenType.RightParen);
    public Token Bang() => Create("!", TokenType.Bang);
    public Token Equal() => Create("=", TokenType.Equal);
    public Token LessThan() => Create("<", TokenType.LessThan);
    public Token GreaterThan() => Create(">", TokenType.GreaterThan);
    public Token LessThanEqual() => Create("<=", TokenType.LessThanEqual);
    public Token GreaterThanEqual() => Create(">=", TokenType.GreaterThanEqual);
    public Token EqualEqual() => Create("==", TokenType.EqualEqual);
    public Token BangEqual() => Create("!=", TokenType.BangEqual);
    public Token AmpAmp() => Create("&&", TokenType.AmpAmp);
    public Token PipePipe() => Create("||", TokenType.PipePipe);
    public Token Eof() => Create("", TokenType.Eof);

    public Token Error(string message) => Create(message, TokenType.Error);

    private Token Create(string lexeme, TokenType type) => new(lexeme, type, _line);
    private Token Create(string lexeme, TokenType type, object? value)  => new(lexeme, type, value, _line);

    public void SetLine(int line) => _line = line;
    public void NextLine() => _line++;

    public Token FromString(string s)
    {
        return s switch
        {
            "+" => Plus(),
            "-" => Minus(),
            "*" => Star(),
            "/" => Slash(),
            "(" => LeftParen(),
            ")" => RightParen(),
            "=" => Equal(),
            "<" => LessThan(),
            ">" => GreaterThan(),
            "<=" => LessThanEqual(),
            ">=" => GreaterThanEqual(),
            "==" => EqualEqual(),
            "!=" => BangEqual(),
            "&&" => AmpAmp(),
            "||" => PipePipe(),
            _ => throw new ArgumentException($"Unknown token to create from '{s}'")
        };
    }
}