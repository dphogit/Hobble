using Hobble.Lang.Interface;
using Hobble.Lang.Lexical;

namespace Hobble.Lang.Parsing;

public class Parser
{
    private class ParseError(string message) : Exception(message);

    /// <summary>The previous token that the scanner just scanned/advanced from.</summary>
    private Token _prev;

    /// <summary>The current token in the source the scanner is on.</summary>
    private Token _current;

    private Scanner _scanner = null!;
    
    private readonly IReporter _reporter;

    public Parser() : this(new ConsoleReporter()) { }

    public Parser(IReporter reporter)
    {
        _reporter = reporter;

        _prev = null!;
        _current = null!;
    }

    /// <summary>Parses the source into an expression tree.</summary>
    /// <param name="source">The expression to parse.</param>
    /// <returns>An expression tree according to the language's grammar.</returns>
    public Expr ParseExpression(string source)
    {
        _scanner = new Scanner(source);
        
        Advance();
        var expr = Expression();
        Consume(TokenType.Eof, "Expected EOF.");

        return expr;
    }

    /// <summary>Parses the source into a statement node.</summary>
    /// <param name="source">The source containing the string to parse.</param>
    /// <returns>A parsed statement node according to the language's grammar.</returns>
    public Stmt ParseStatement(string source)
    {
        _scanner = new Scanner(source);
        
        Advance();
        var stmt = Declaration();
        Consume(TokenType.Eof, "Expected EOF.");
        
        return stmt;
    }

    #region Statement Grammar

    private Stmt Declaration()
    {
        if (Match(TokenType.Var))
            return VarDecl();

        return Statement();
    }

    private VarStmt VarDecl()
    {
        var identifier = Consume(TokenType.Identifier, "Expect variable name.");
        var initializer = Match(TokenType.Equal) ? Expression() : null;
        ConsumeEndOfStatementSemiColon();
        return new VarStmt(identifier, initializer);
    }
    
    private Stmt Statement()
    {
        if (Match(TokenType.Print))
            return PrintStmt();

        return ExprStmt();
    }

    private PrintStmt PrintStmt()
    {
        var expr = Expression();
        ConsumeEndOfStatementSemiColon();
        return new PrintStmt(expr);
    }

    private ExprStmt ExprStmt()
    {
        var expr = Expression();
        ConsumeEndOfStatementSemiColon();
        return new ExprStmt(expr);
    }
    
    #endregion

    #region Expression Grammar 

    private Expr Expression()
    {
        return LogicalOr();
    }

    private Expr LogicalOr()
    {
        return LeftAssociativeBinaryOperator(LogicalAnd, TokenType.PipePipe);
    }

    private Expr LogicalAnd()
    {
        return LeftAssociativeBinaryOperator(Equality, TokenType.AmpAmp);
    }

    private Expr Equality()
    {
        return LeftAssociativeBinaryOperator(Relational, TokenType.EqualEqual, TokenType.BangEqual);
    }

    private Expr Relational()
    {
        return LeftAssociativeBinaryOperator(
            Additive,
            TokenType.LessThan, TokenType.GreaterThan, TokenType.LessThanEqual, TokenType.GreaterThanEqual);
    }

    private Expr Additive()
    {
        return LeftAssociativeBinaryOperator(Multiplicative, TokenType.Plus, TokenType.Minus);
    }

    private Expr Multiplicative()
    {
        return LeftAssociativeBinaryOperator(Unary, TokenType.Star, TokenType.Slash);
    }

    private Expr Unary()
    {
        if (Match(TokenType.Minus, TokenType.Bang))
        {
            var op = _prev;
            var right = Unary();
            return new UnaryExpr(op, right);
        }

        return Primary();
    }

    private Expr Primary()
    {
        if (Match(TokenType.Number))
            return LiteralExpr.Number(_prev.GetNumberValue());

        if (Match(TokenType.String))
            return LiteralExpr.String(_prev.GetStringValue());

        if (Match(TokenType.LeftParen))
        {
            var expr = Expression();
            Consume(TokenType.RightParen, "Expected closing ')' at end of expression.");
            return expr;
        }

        if (Match(TokenType.True))
            return LiteralExpr.True();
        
        if (Match(TokenType.False))
            return LiteralExpr.False();

        if (Match(TokenType.Identifier))
            return new VarExpr(_prev);

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

    #endregion

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

    private Token Consume(TokenType type, string failureMessage)
    {
        if (Check(type))
        {
            Advance();
            return _prev;
        }
        
        throw CreateParseError(failureMessage);
    }

    private void ConsumeEndOfStatementSemiColon()
    {
        Consume(TokenType.SemiColon, "Expected ';' at end of statement.");
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
        _reporter.Error(_current, message);
        return new ParseError(message);
    }
}