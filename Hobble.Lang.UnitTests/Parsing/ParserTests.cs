using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;

namespace Hobble.Lang.UnitTests.Parsing;

public class ParserTests
{
    private readonly TokenFactory _tokenFactory = new();

    #region Expression Parsing
    
    [Theory]
    [InlineData("1 + 2", 1, "+", 2)]
    [InlineData("3 - 4", 3, "-", 4)]
    [InlineData("5 * 6", 5, "*", 6)]
    [InlineData("7 / 8", 7, "/", 8)]
    [InlineData("9 < 10", 9, "<", 10)]
    [InlineData("11 <= 12", 11, "<=", 12)]
    [InlineData("13 == 14", 13, "==", 14)]
    [InlineData("15 > 16", 15, ">", 16)]
    [InlineData("17 >= 18", 17, ">=", 18)]
    [InlineData("19 != 20", 19, "!=", 20)]
    public void ParseExpression_NumericBinaryOperations_ReturnsBinaryExpr(string source, int left, string op, int right)
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression(source);

        var expectedExpr = new BinaryExpr(
            LiteralExpr.Number(left),
            _tokenFactory.FromString(op),
            LiteralExpr.Number(right));
        Assert.Equal(expectedExpr, expr);
    }

    [Theory]
    [InlineData("true && true", true, "&&", true)]
    [InlineData("true || false", true, "||", false)]
    public void ParseExpression_LogicalOperations_ReturnsBinaryExpr(string source, bool left, string op, bool right)
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression(source);

        var expectedExpr = new BinaryExpr(
            LiteralExpr.Bool(left),
            _tokenFactory.FromString(op),
            LiteralExpr.Bool(right));
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_NegateNumber_ReturnsUnaryExpr()
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression("-1");

        var expectedExpr = new UnaryExpr(_tokenFactory.Minus(), LiteralExpr.Number(1));
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_NegateNegativeNumber_ReturnsNestedUnaryExpr()
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression("--1");
        
        var nestedExpr = new UnaryExpr(_tokenFactory.Minus(), LiteralExpr.Number(1));
        var expectedExpr = new UnaryExpr(_tokenFactory.Minus(), nestedExpr);
        Assert.Equal(expectedExpr, expr);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ParseExpression_Boolean_ReturnsLiteral(bool operand)
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression(operand.ToString().ToLower());
        
        var expectedExpr = LiteralExpr.Bool(operand); 
        Assert.Equal(expectedExpr, expr);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ParseExpression_LogicalNot_ReturnsUnaryExpr(bool operand)
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression($"!{operand}".ToLower());
        
        var expectedExpr = new UnaryExpr(_tokenFactory.Bang(), LiteralExpr.Bool(operand));
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_MultiplePrecedence_ReturnsCorrectPrecedenceExpr()
    {
        var parser = new Parser();

        var expr = parser.ParseExpression("1 + 2 * 3");
        
        var expectedLeft = LiteralExpr.Number(1);
        var expectedRight = new BinaryExpr(LiteralExpr.Number(2), _tokenFactory.Star(), LiteralExpr.Number(3));
        var expectedExpr = new BinaryExpr(expectedLeft, _tokenFactory.Plus(), expectedRight);
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_ContainsParenthesis_ReturnsCorrectPrecedenceExpr()
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression("(1 + 2) * 3");
        
        var innerLeft = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.Plus(), LiteralExpr.Number(2));
        var expectedLeft = new GroupExpr(innerLeft);
        var expectedRight = LiteralExpr.Number(3);
        var expectedExpr = new BinaryExpr(expectedLeft, _tokenFactory.Star(), expectedRight);
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_LeftAssociative_ReturnsCorrectOrderExpr()
    {
        var parser = new Parser();

        var expr = parser.ParseExpression("1 + 2 + 3");
        
        var expectedLeft = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.Plus(), LiteralExpr.Number(2));
        var expectedRight = LiteralExpr.Number(3);
        var expectedExpr = new BinaryExpr(expectedLeft, _tokenFactory.Plus(), expectedRight);
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_Identifier_ReturnsVarExpr()
    {
        const string identifier = "age";
        var parser = new Parser();
        
        var expr = parser.ParseExpression(identifier);
        
        var expectedExpr = new VarExpr(_tokenFactory.Identifier(identifier));
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_Assignment_ReturnsAssignExpr()
    {
        var parser = new Parser();

        var expr = parser.ParseExpression("x = 10");

        var expectedExpr = new AssignExpr(_tokenFactory.Identifier("x"), LiteralExpr.Number(10));
        Assert.Equal(expectedExpr, expr);
    }

    [Fact]
    public void ParseExpression_FunctionCall_ReturnsCallExpr()
    {
        var parser = new Parser();

        var expr = parser.ParseExpression("printSum(1, 2)");

        var callee = new VarExpr(_tokenFactory.Identifier("printSum"));
        var expectedExpr = new CallExpr(callee, [LiteralExpr.Number(1), LiteralExpr.Number(2)]);
        Assert.Equal(expectedExpr, expr);
    }

    #endregion
    
    #region Statement Parsing

    [Fact]
    public void ParseStatement_PrintKeyword_ReturnsPrintStmt()
    {
        var parser = new Parser();
        
        var stmt = parser.ParseStatement("print true;");

        var expectedStmt = new PrintStmt(LiteralExpr.True());
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_ExpressionStatement_ReturnsExprStmt()
    {
        var parser = new Parser();
        
        var stmt = parser.ParseStatement("true;");
        
        var expectedStmt = new ExprStmt(LiteralExpr.True());
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_VariableDeclarationNoInitializer_ReturnsVarStmt()
    {
        var parser = new Parser();
        
        var stmt = parser.ParseStatement("var x;");

        var expectedStmt = new VarStmt(_tokenFactory.Identifier("x"));
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_VariableDeclarationInitializer_ReturnsVarStmt()
    {
        var parser = new Parser();
        
        var stmt = parser.ParseStatement("var x = 67;");

        var expectedStmt = new VarStmt(_tokenFactory.Identifier("x"), LiteralExpr.Number(67));
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_EmptyBlock_ReturnsBlockStmt()
    {
        var parser = new Parser();

        var stmt = parser.ParseStatement("{}");

        var expectedStmt = new BlockStmt([]);
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_If_ReturnsIfStmt()
    {
        var parser = new Parser();

        var stmt = parser.ParseStatement("if (true) print 1;");

        var expectedStmt = new IfStmt(LiteralExpr.True(), new PrintStmt(LiteralExpr.Number(1)));
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_IfElse_ReturnsIfStmt()
    {
        var parser  = new Parser();
        
        var stmt = parser.ParseStatement("if (true) print 1; else print 2;");

        var expectedStmt = new IfStmt(
            LiteralExpr.True(),
            new PrintStmt(LiteralExpr.Number(1)),
            new PrintStmt(LiteralExpr.Number(2)));
        
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_While_ReturnsWhileStmt()
    {
        var parser  = new Parser();

        var stmt = parser.ParseStatement("while (true) print 1;");
        
        var expectedStmt = new WhileStmt(LiteralExpr.True(), new PrintStmt(LiteralExpr.Number(1)));
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_ForLoop_DesugarsIntoBlockAndWhileStmt()
    {
        var parser = new Parser();

        var stmt = parser.ParseStatement("for (var i = 0; i < 3; i = i + 1) print i;");
        
        /*
         * The for loop should desugar into:
         * {
         *   var i = 0;         // initializer
         *   while (i < 3) {    // a block is created to capture the existing body statement and increment
         *     print i;
         *     i = i + 1; 
         *   }
         * }
         */

        var i = _tokenFactory.Identifier("i");
        var iExpr = new VarExpr(i);
        var initializer = new VarStmt(i, LiteralExpr.Number(0));
        var condition = new BinaryExpr(iExpr, _tokenFactory.LessThan(), LiteralExpr.Number(3));
        var increment = new AssignExpr(i, new BinaryExpr(iExpr, _tokenFactory.Plus(), LiteralExpr.Number(1)));
        var whileBody = new BlockStmt([new PrintStmt(iExpr), new ExprStmt(increment)]);
        var expectedStmt = new BlockStmt([initializer, new WhileStmt(condition, whileBody)]);
        
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_FunctionDeclaration_ReturnsFnStmt()
    {
        var parser  = new Parser();

        var stmt = parser.ParseStatement("fn printSum(a, b) { print a + b; }");

        var a = _tokenFactory.Identifier("a");
        var b = _tokenFactory.Identifier("b");
        var printStmt = new PrintStmt(new BinaryExpr(new VarExpr(a), _tokenFactory.Plus(), new VarExpr(b)));
        var expectedStmt = new FnStmt(_tokenFactory.Identifier("printSum"), [a, b], new BlockStmt([printStmt]));
        Assert.Equal(expectedStmt, stmt);
    }

    [Fact]
    public void ParseStatement_ReturnExpression_ReturnsReturnStmt()
    {
        var parser = new Parser();
        
        var stmt = parser.ParseStatement("return 67;");
        
        var expectedStmt = new ReturnStmt(_tokenFactory.Return(), LiteralExpr.Number(67));
        Assert.Equal(expectedStmt, stmt);
    }
    
    #endregion

    #region Error Handling

    [Fact]
    public void ParseProgram_NoSemiColonEndOfStatement_FlagsError()
    {
        var parser = new Parser();

        var parseTree = parser.ParseProgram("var x = 10");
        
        Assert.True(parser.HadError);
        Assert.Empty(parseTree.Stmts);
    }

    [Fact]
    public void ParseProgram_InvalidToken_FlagsError()
    {
        var parser = new Parser();

        var parseTree = parser.ParseProgram("$");
        
        Assert.True(parser.HadError);
        Assert.Empty(parseTree.Stmts);
    }

    [Fact]
    public void ParseProgram_NoClosingBraceEndOfBlock_FlagsError()
    {
        var parser = new Parser();

        var parseTree = parser.ParseProgram("{");
        
        Assert.True(parser.HadError);
        Assert.Empty(parseTree.Stmts);
    }

    [Fact]
    public void ParseProgram_MissingOperand_FlagsError()
    {
        var parser = new Parser();
        
        var parseTree = parser.ParseProgram("1 + ");
        
        Assert.True(parser.HadError);
        Assert.Empty(parseTree.Stmts);
    }

    #endregion
}