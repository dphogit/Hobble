using Hobble.Lang.Interpreter;
using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;
using Hobble.Lang.Representation;

namespace Hobble.Lang.UnitTests.Parsing;

public class TreeWalkInterpreterTests
{
    private readonly TokenFactory _tokenFactory = new();
    
    [Theory]
    [InlineData(1, '+', 2, 3)]
    [InlineData(4, '-', 2, 2)]
    [InlineData(3, '*', 2, 6)]
    [InlineData(10, '/', 2, 5)]
    public void Evaluate_BinaryExpressions_EvaluatesCorrectly(int a, char op, int b, int expected)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(a), _tokenFactory.FromChar(op), LiteralExpr.Number(b));

        var result = interpreter.Evaluate(expr);

        var expectedResult = HobbleValue.Number(expected);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Evaluate_MixedPrecedence_EvaluatesCorrectly()
    {
        var interpreter = new TreeWalkInterpreter();

        // 6 / 3 - 1 = 1
        var left = new BinaryExpr(LiteralExpr.Number(6), _tokenFactory.Slash(), LiteralExpr.Number(3));
        var right = LiteralExpr.Number(1);
        var expr = new BinaryExpr(left, _tokenFactory.Minus(), right);
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.Number(1);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Evaluate_AddTwoStrings_Concatenates()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.String("Hello, "), _tokenFactory.Plus(), LiteralExpr.String("World!"));
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.String("Hello, World!");
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Evaluate_DivideByZero_ThrowsDivideByZeroError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.Slash(), LiteralExpr.Number(0));
        
        Assert.Throws<DivideByZeroError>(() => interpreter.Evaluate(expr));
    }

    [Fact]
    public void Evaluate_AddNumberAndString_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.Plus(), LiteralExpr.String("1"));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
    }
}