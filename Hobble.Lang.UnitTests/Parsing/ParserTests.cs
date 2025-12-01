using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;

namespace Hobble.Lang.UnitTests.Parsing;

public class ParserTests
{
    private readonly TokenFactory _tokenFactory = new();
    
    [Theory]
    [InlineData("1 + 2", 1, '+', 2)]
    [InlineData("3 - 4", 3, '-', 4)]
    [InlineData("5 * 6", 5, '*', 6)]
    [InlineData("7 / 8", 7, '/', 8)]
    public void ParseExpression_BinaryOperations_ReturnsBinaryExpr(string source, int left, char op, int right)
    {
        var parser = new Parser();
        
        var expr = parser.ParseExpression(source);

        var expectedExpr = new BinaryExpr(
            LiteralExpr.Number(left),
            _tokenFactory.FromChar(op),
            LiteralExpr.Number(right));

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
}