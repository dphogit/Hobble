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

    /// <summary>True when an error is detected after parsing the program.</summary>
    public bool HadError;

    public Parser() : this(new ConsoleReporter()) { }

    public Parser(IReporter reporter)
    {
        _reporter = reporter;

        _prev = null!;
        _current = null!;
    }

    /// <summary>Parses the source into the parse tree representing the program's syntactical structure.</summary>
    /// <param name="source">The program source to parse.</param>
    /// <returns>A parse tree representing the program.</returns>
    public ParseTree ParseProgram(string source)
    {
        List<Stmt> stmts = [];
        
        InitScanner(source);        

        while (!Match(TokenType.Eof))
        {
            try
            {
                var stmt = Declaration();
                stmts.Add(stmt);
            }
            catch (ParseError)
            {
                HadError = true;
                Synchronize();
            }
        }

        return new ParseTree(stmts);
    }

    /// <summary>Parses the source into an expression tree.</summary>
    /// <param name="source">The expression to parse.</param>
    /// <returns>An expression tree according to the language's grammar.</returns>
    public Expr ParseExpression(string source)
    {
        InitScanner(source);
        
        var expr = Expression();
        Consume(TokenType.Eof, "Expected EOF.");

        return expr;
    }

    /// <summary>Parses the source into a statement node.</summary>
    /// <param name="source">The source containing the string to parse.</param>
    /// <returns>A parsed statement node according to the language's grammar.</returns>
    public Stmt ParseStatement(string source)
    {
        InitScanner(source);
        
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

        if (Match(TokenType.LeftBrace))
            return Block();

        if (Match(TokenType.If))
            return IfStmt();

        return ExprStmt();
    }

    private PrintStmt PrintStmt()
    {
        var expr = Expression();
        ConsumeEndOfStatementSemiColon();
        return new PrintStmt(expr);
    }

    private BlockStmt Block()
    {
        List<Stmt> stmts = [];
        
        while (!Check(TokenType.RightBrace) && _current.Type != TokenType.Eof)
        {
            stmts.Add(Declaration());
        }

        Consume(TokenType.RightBrace, "Expected '}' at end of block.");
        return new BlockStmt(stmts);
    }

    private IfStmt IfStmt()
    {
        Consume(TokenType.LeftParen, "Expect '(' after 'if'.");
        var condition = Expression();
        Consume(TokenType.RightParen, "Expect ')' after if condition.");
        
        var thenBranch = Statement();
        var elseBranch = Match(TokenType.Else) ? Statement() : null;
        
        return new IfStmt(condition, thenBranch, elseBranch);
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
        return Assignment();
    }

    private Expr Assignment()
    {
        var expr = LogicalOr();

        if (!Match(TokenType.Equal))
            return expr;

        // Assignment detected, convert expr which was an r-value into an l-value (assignment target).
        // This conversion is only valid if the assignment target is valid (a VarExpr syntax node).
        
        var assignOp = _prev;
        var value = Assignment();
        
        return expr is VarExpr varExpr
            ? new AssignExpr(varExpr.Identifier, value)
            : throw CreateParseError(assignOp, "Invalid assignment target");
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
            return new GroupExpr(expr);
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

    private void InitScanner(string source)
    {
        _scanner = new Scanner(source);
        Advance();
    }
    
    private void Advance()
    {
        _prev = _current;

        while (true)
        {
            _current = _scanner.NextToken();

            if (_current.Type != TokenType.Error)
                return;

            HadError = true;
            _reporter.Error(_current, _current.Lexeme);
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
        return CreateParseError(_current, message);
    }

    private ParseError CreateParseError(Token token, string message)
    {
        _reporter.Error(token, message);
        return new ParseError(message);
    }

    /// <summary>
    /// Moves forward and discards tokens until next statement boundary. This gives a good user experience in trying to
    /// give back as many errors as possible at once rather than one at a time, while also a best effort at preventing
    /// errors cascading into future separate errors.
    /// </summary>
    private void Synchronize()
    {
        while (_current.Type != TokenType.Eof)
        {
            _prev = _current;
            _current = _scanner.NextToken();

            if (_prev.Type == TokenType.SemiColon)
                return;
            
            if (Keywords.IsStatementStarter(_current.Type))
                return;
        }
    }
}