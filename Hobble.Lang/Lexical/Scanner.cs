namespace Hobble.Lang.Lexical;

public class Scanner(string source)
{
    /// <summary>Index of the starting character of the token being scanned within the source.</summary>
    private int _start = 0;

    /// <summary>Index of the current character to be scanned in the source.</summary>
    private int _current = 0;

    private readonly TokenFactory _tokenFactory = new();

    /// <summary>Scans the next token in the source.</summary>
    /// <returns>The next token in the source.</returns>
    /// <remarks>
    /// The user should check for errors such as unexpected characters or invalid character sequences by checking
    /// the <see cref="TokenType"/> of the returned token, specifically for type <see cref="TokenType.Error"/>.
    /// If an error is returned, then the message can be obtained by the token's <see cref="Token.Lexeme"/> property.
    /// </remarks>
    public Token NextToken()
    {
        if (AtEnd())
            return _tokenFactory.Eof();
        
        SkipWhitespace();

        if (AtEnd())
            return _tokenFactory.Eof();

        _start = _current;
        
        var c = Advance();

        if (IsHobbleDigit(c))
            return Number();

        if (IsHobbleAlpha(c))
            return Identifier();

        return c switch
        {
            '+' => _tokenFactory.Plus(),
            '-' => _tokenFactory.Minus(),
            '*' => _tokenFactory.Star(),
            '/' => _tokenFactory.Slash(),
            '(' => _tokenFactory.LeftParen(),
            ')' => _tokenFactory.RightParen(),
            '!' => Match('=') ? _tokenFactory.BangEqual() : _tokenFactory.Bang(),
            '<' => Match('=') ? _tokenFactory.LessThanEqual() : _tokenFactory.LessThan(),
            '>' => Match('=') ? _tokenFactory.GreaterThanEqual() : _tokenFactory.GreaterThan(),
            '=' => Match('=') ? _tokenFactory.EqualEqual() : _tokenFactory.Equal(),
            '&' => Match('&') ? _tokenFactory.AmpAmp() : _tokenFactory.Error("Logical AND requires two ampersands."),
            '|' => Match('|') ? _tokenFactory.PipePipe() : _tokenFactory.Error("Logical OR requires two ampersands."),
            ';' => _tokenFactory.SemiColon(),
            '"' => String(),
            _ => _tokenFactory.Error($"Unexpected character '{c}'.")
        };
    }

    private Token Number()
    {
        ConsumeDigits();

        // Handle fractional part of the number if it exists.
        if (!AtEnd() && Peek() == '.')
        {
            Advance();

            if (AtEnd() || !IsHobbleDigit(Peek()))
                return _tokenFactory.Error("Digits are expected after '.' for numbers.");

            ConsumeDigits();
        }

        var lexeme = source.Substring(_start, _current - _start);
        var value = decimal.Parse(lexeme);
        return _tokenFactory.Number(lexeme, value);

        void ConsumeDigits()
        {
            while (!AtEnd() && IsHobbleDigit(Peek()))
                Advance();
        }
    }

    private Token Identifier()
    {
        while (!AtEnd() && IsHobbleAlpha(Peek()))
            Advance();
        
        var lexeme = source.Substring(_start, _current - _start);

        return Keywords.TryGetTokenType(lexeme, out var type)
            ? _tokenFactory.Keyword(lexeme, type.Value)
            : _tokenFactory.Identifier(lexeme);
    }

    private Token String()
    {
        while (!AtEnd() && Peek() != '"')
            Advance();

        if (AtEnd())
            return _tokenFactory.Error("Unterminated string.");

        Advance();

        var value = source.Substring(_start + 1, _current - _start - 2);
        return _tokenFactory.String(value);
    }

    private void SkipWhitespace()
    {
        while (true)
        {
            switch (Peek())
            {
                case '/':
                    if (PeekNext() != '/')
                        return;
                    InlineComment();
                    continue;
                case '\n':
                    NextLine();
                    continue;
                case ' ':
                case '\t':
                case '\r':
                    Advance();
                    continue;
                default:
                    return;
            }
        }

        void InlineComment()
        {
            while (!AtEnd() && Peek() != '\n')
                Advance();
        }
    }

    private char Peek()
    {
        return _current >= source.Length ? '\0' : source[_current];
    }

    private char PeekNext()
    {
        return _current + 1 >= source.Length ? '\0' : source[_current + 1];
    }

    /// <summary>Advance the scanner's current position one character forward.</summary>
    /// <returns>The recent character the scanner was currently on before advancing.</returns>
    private char Advance()
    {
        var c = Peek();
        _current++;
        return c;
    }

    /// <summary>Checks the current character of the scanner and advances the scanner if it matches.</summary>
    /// <param name="c">The expected character to match on.</param>
    /// <returns>True if the current character the scanner is on matches the passed character.</returns>
    private bool Match(char c)
    {
        if (c != Peek())
            return false;
        
        Advance();
        return true;
    }

    private void NextLine()
    {
        Advance();
        _tokenFactory.NextLine();
    }

    private bool AtEnd()
    {
        return _current >= source.Length;
    }

    private static bool IsHobbleDigit(char c)
    {
        return char.IsAsciiDigit(c);
    }

    private static bool IsHobbleAlpha(char c)
    {
        return char.IsAsciiLetter(c) || c == '_';
    }
}