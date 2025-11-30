using Hobble.Lang.Interface;
using Hobble.Lang.Lexical;

namespace Hobble.Lang.Parsing;

public class Parser(IReporter reporter)
{
    private class ParseError(string message) : Exception(message);
    
    /// <summary>The previous token that the scanner just scanned/advanced from.</summary>
    private Token _prev = null!;
    
    /// <summary>The current token in the source the scanner is on.</summary>
    private Token _current = null!;

    private Scanner _scanner = null!;

    public Parser() : this(new ConsoleReporter()) { }
    
    /// <summary>Parses the source into an expression tree.</summary>
    /// <param name="source">The expression to parse.</param>
    /// <returns>An expression tree according to the language's grammar.</returns>
    public Expr ParseExpression(string source)
    {
        _prev = null!;
        _current = null!;
        _scanner = new Scanner(source);
        
        Advance();
        var expr = Expression();
        Consume(TokenType.Eof, "Expected end of expression.");

        return expr;
    }

    private Expr Expression()
    {
        return Additive();
    }

    private Expr Additive()
    {
        return LeftAssociativeBinaryOperator(Multiplicative, TokenType.Plus, TokenType.Minus);
    }

    private Expr Multiplicative()
    {
        return LeftAssociativeBinaryOperator(Primary, TokenType.Star, TokenType.Slash);
    }

    private Expr Primary()
    {
        if (Match(TokenType.Number))
            return LiteralExpr.Number(_prev.GetNumberValue());

        if (Match(TokenType.String))
            return LiteralExpr.String(_prev.GetStringValue());

        throw CreateParseError("Expected expression.");
    }

    private Expr LeftAssociativeBinaryOperator(Func<Expr> parseOperand, params TokenType[] types)
    {
        var expr = parseOperand();

        while (Match(types))
        {
            var op = _prev;
            var right = parseOperand();
            expr = new BinaryExpr(expr, op, right);
        }
        
        return expr;
    }

    private void Advance()
    {
        _prev = _current;

        while (true)
        {
            _current = _scanner.NextToken();

            if (_current.Type == TokenType.Error)
            {
                throw CreateParseError(_current.Lexeme);
            }

            return;
        }
    }

    private void Consume(TokenType type, string failureMessage)
    {
        if (Check(type))
        {
            Advance();
            return;
        }
        
        throw CreateParseError(failureMessage);
    }

    private bool Check(TokenType type)
    {
        return _current.Type == type;
    }
    
    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check))
            return false;
        
        Advance();
        return true;
    }

    private ParseError CreateParseError(string message)
    {
        reporter.Error(_current, message);
        return new ParseError(message);
    }
}