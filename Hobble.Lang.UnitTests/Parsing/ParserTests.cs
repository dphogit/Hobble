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
        
        var expectedLeft = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.Plus(), LiteralExpr.Number(2));
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

    #endregion
}