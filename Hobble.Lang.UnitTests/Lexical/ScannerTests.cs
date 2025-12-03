using Hobble.Lang.Lexical;

namespace Hobble.Lang.UnitTests.Lexical;

public class ScannerTests
{
    [Theory]
    [InlineData("+", TokenType.Plus)]
    [InlineData("-", TokenType.Minus)]
    [InlineData("*", TokenType.Star)]
    [InlineData("/", TokenType.Slash)]
    [InlineData("(", TokenType.LeftParen)]
    [InlineData(")", TokenType.RightParen)]
    [InlineData("!", TokenType.Bang)]
    [InlineData("=", TokenType.Equal)]
    [InlineData("<", TokenType.LessThan)]
    [InlineData(">", TokenType.GreaterThan)]
    [InlineData("<=", TokenType.LessThanEqual)]
    [InlineData(">=", TokenType.GreaterThanEqual)]
    [InlineData("==", TokenType.EqualEqual)]
    [InlineData("!=", TokenType.BangEqual)]
    [InlineData("&&", TokenType.AmpAmp)]
    [InlineData("||", TokenType.PipePipe)]
    [InlineData(";", TokenType.SemiColon)]
    [InlineData("false", TokenType.False)]
    [InlineData("true", TokenType.True)]
    [InlineData("print", TokenType.Print)]
    [InlineData("var", TokenType.Var)]
    [InlineData("identifier", TokenType.Identifier)]
    [InlineData("", TokenType.Eof)]
    public void NextToken_NonValueTokenTypes_ReturnsToken(string lexeme, TokenType type)
    {
        var scanner = new Scanner(lexeme);
        var expectedToken = new Token(lexeme, type);
        
        var token  = scanner.NextToken();
        
        Assert.Equal(expectedToken, token);
    }

    [Theory]
    [InlineData("    +", "+", TokenType.Plus, 1)]
    [InlineData("\t+", "+", TokenType.Plus, 1)]
    [InlineData("\r+", "+", TokenType.Plus, 1)]
    [InlineData("\n+", "+", TokenType.Plus, 2)]
    public void NextToken_LeadingWhitespace_ReturnsToken(string source, string lexeme, TokenType type, int line)
    {
        var scanner = new Scanner(source);
        var expectedToken = new Token(lexeme, type, line);
        
        var token  = scanner.NextToken();
        
        Assert.Equal(expectedToken, token);
    }

    [Theory]
    [InlineData("+ // This is an inline comment", "+", TokenType.Plus, 1)]
    [InlineData("// This is an inline comment\n+", "+", TokenType.Plus, 2)]
    public void NextToken_Comments_ReturnsToken(string source, string lexeme, TokenType type, int line)
    {
        var scanner = new Scanner(source);
        var expectedToken = new Token(lexeme, type, line);
        
        var token  = scanner.NextToken();
        
        Assert.Equal(expectedToken, token);
    }

    [Theory]
    [InlineData("67")]
    [InlineData("6.7")]
    [InlineData("6.767")]
    public void NextToken_Numbers_ReturnsToken(string lexeme)
    {
        var value = decimal.Parse(lexeme);
        NextToken_ValueTypes_ReturnToken(lexeme, TokenType.Number, value);
    }

    [Theory]
    [InlineData("\"Hello, World!\"", "Hello, World!")]
    [InlineData("\"First Line.\nSecond Line.\"", "First Line.\nSecond Line.")]
    [InlineData("\"\"", "")]
    public void NextToken_String_ReturnsToken(string lexeme, string value)
    {
        NextToken_ValueTypes_ReturnToken(lexeme, TokenType.String, value);
    }
    
    [Theory]
    [InlineData("123.", "Digits are expected after '.' for numbers.")]
    [InlineData("#", "Unexpected character '#'.")]
    [InlineData("\"a", "Unterminated string.")]
    public void NextToken_InvalidSource_ReturnErrorTokens(string source, string message)
    {
        var scanner = new Scanner(source);

        var token = scanner.NextToken();
        
        Assert.Equal(message, token.Lexeme);
    }
    
    private static void NextToken_ValueTypes_ReturnToken(string lexeme, TokenType type, object? value)
    {
        var scanner = new Scanner(lexeme);
        var expectedToken = new Token(lexeme, type, value);
        
        var token  = scanner.NextToken();
        
        Assert.Equal(expectedToken, token);
    }
}