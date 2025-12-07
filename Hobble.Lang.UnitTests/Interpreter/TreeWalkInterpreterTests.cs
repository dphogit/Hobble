using Hobble.Lang.Interface;
using Hobble.Lang.Interpreter;
using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;
using Hobble.Lang.Representation;
using NSubstitute;

namespace Hobble.Lang.UnitTests.Interpreter;

public class TreeWalkInterpreterTests
{
    private readonly TokenFactory _tokenFactory = new();

    #region Expression Evaluation
    
    [Theory]
    [InlineData(1, "+", 2, 3)]
    [InlineData(4, "-", 2, 2)]
    [InlineData(3, "*", 2, 6)]
    [InlineData(10, "/", 2, 5)]
    public void Evaluate_ArithmeticExpressions_EvaluatesCorrectly(int a, string op, int b, int expected)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(a), _tokenFactory.FromString(op), LiteralExpr.Number(b));

        var result = interpreter.Evaluate(expr);

        var expectedResult = HobbleValue.Number(expected);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(1, "<", 2, true)]
    [InlineData(2, "<", 1, false)]
    [InlineData(3, "<=", 4, true)]
    [InlineData(4, "<=", 4, true)]
    [InlineData(4, "<=", 3, false)]
    [InlineData(5, ">", 6, false)]
    [InlineData(6, ">", 5, true)]
    [InlineData(7, ">=", 8, false)]
    [InlineData(8, ">=", 8, true)]
    [InlineData(8, ">=", 7, true)]
    [InlineData(9, "==", 9, true)]
    [InlineData(9, "==", 10, false)]
    [InlineData(11, "!=", 11, false)]
    [InlineData(11, "!=", 12, true)]
    public void Evaluate_RelationalExpressions_EvaluatesToCorrectBoolean(int a, string op, int b, bool expected)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr  = new BinaryExpr(LiteralExpr.Number(a), _tokenFactory.FromString(op), LiteralExpr.Number(b));
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.Bool(expected);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(false, "&&", false, false)]
    [InlineData(false, "&&", true, false)]
    [InlineData(true, "&&", false, false)]
    [InlineData(true, "&&", true, true)]
    [InlineData(false, "||", false, false)]
    [InlineData(false, "||", true, true)]
    [InlineData(true, "||", false, true)]
    [InlineData(true, "||", true, true)]
    public void Evaluate_LogicalExpressions_EvaluatesToCorrectBoolean(bool a, string op, bool b, bool expected)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Bool(a), _tokenFactory.FromString(op), LiteralExpr.Bool(b));
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.Bool(expected);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(-1)]
    public void Evaluate_NegativeSignInFrontOfNumber_EvaluatesToNumericNegation(int value)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new UnaryExpr(_tokenFactory.Minus(), LiteralExpr.Number(value));
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.Number(-value);
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Evaluate_BangInFrontOfBool_EvaluatesToLogicalNegation(bool b)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new UnaryExpr(_tokenFactory.Bang(), LiteralExpr.Bool(b));
        
        var result = interpreter.Evaluate(expr);

        var expectedResult = HobbleValue.Bool(!b);
        Assert.Equal(expectedResult, result);
    }
    
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Evaluate_DoubleBangInFrontOfBool_EvaluatesToSameValue(bool b)
    {
        var interpreter = new TreeWalkInterpreter();
        var innerExpr = new UnaryExpr(_tokenFactory.Bang(), LiteralExpr.Bool(b));
        var expr = new UnaryExpr(_tokenFactory.Bang(), innerExpr);
        
        var result = interpreter.Evaluate(expr);

        var expectedResult = HobbleValue.Bool(b);
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
    public void Evaluate_RaisedPrecedenceFromParenthesis_EvaluatesInCorrectOrder()
    {
        var interpreter = new TreeWalkInterpreter();
        
        // 6 / (3 - 1) = 3
        var left = LiteralExpr.Number(6);
        var rightInner = new BinaryExpr(LiteralExpr.Number(3), _tokenFactory.Slash(), LiteralExpr.Number(1));
        var right = new GroupExpr(rightInner);
        var expr  = new BinaryExpr(left, _tokenFactory.Minus(), right);
        
        var result = interpreter.Evaluate(expr);
        
        var expectedResult = HobbleValue.Number(3);
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
    public void Evaluate_NegateNonNumber_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new UnaryExpr(_tokenFactory.Minus(), LiteralExpr.String("1"));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
    }

    [Fact]
    public void Evaluate_LogicalNegateNonBool_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new UnaryExpr(_tokenFactory.Bang(), LiteralExpr.Number(0));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
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

    [Theory]
    [InlineData("<")]
    [InlineData("<=")]
    [InlineData(">")]
    [InlineData(">=")]
    [InlineData("==")]
    [InlineData("!=")]
    public void Evaluate_RelationalOperatorsNonNumberArgs_ThrowsRuntimeError(string op)
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.String("2"), _tokenFactory.FromString(op), LiteralExpr.Number(1));
        var expr2 = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.FromString(op), LiteralExpr.String("2"));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr2));
    }

    [Fact]
    public void Evaluate_LogicalAndNonBoolArgs_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.AmpAmp(), LiteralExpr.True());
        var expr2 = new BinaryExpr(LiteralExpr.True(), _tokenFactory.AmpAmp(), LiteralExpr.Number(1));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr2));
    }
    
    [Fact]
    public void Evaluate_LogicalOrNonBoolArgs_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var expr = new BinaryExpr(LiteralExpr.Number(1), _tokenFactory.PipePipe(), LiteralExpr.True());
        var expr2 = new BinaryExpr(LiteralExpr.False(), _tokenFactory.PipePipe(), LiteralExpr.Number(1));
        
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr));
        Assert.Throws<RuntimeError>(() => interpreter.Evaluate(expr2));
    }
    
    [Fact]
    public void Evaluate_Variable_RetrievesAssociatedValue()
    {
        var identifier = _tokenFactory.Identifier("age");
        var interpreter = new TreeWalkInterpreter();
        var varDecl = new VarStmt(identifier, LiteralExpr.Number(67));
        interpreter.Execute(varDecl);

        var result = interpreter.Evaluate(new VarExpr(identifier));

        var expectedResult = HobbleValue.Number(67);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Evaluate_Assignment_ReturnsUpdatedValue()
    {
        var identifier = _tokenFactory.Identifier("age");
        var interpreter = new TreeWalkInterpreter();
        var varDecl = new VarStmt(identifier, LiteralExpr.Number(67));
        interpreter.Execute(varDecl);

        var result = interpreter.Evaluate(new AssignExpr(identifier, LiteralExpr.Number(69)));
        
        var expectedResult = HobbleValue.Number(69);
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public void Evaluate_AssignmentUndefinedVariable_ThrowsRuntimeError()
    {
        var interpreter = new TreeWalkInterpreter();
        var assignExpr = new AssignExpr(_tokenFactory.Identifier("age"), LiteralExpr.Number(67));

        Assert.ThrowsAny<RuntimeError>(() => interpreter.Evaluate(assignExpr));
    }

    #endregion

    #region Statement Execution
    
    [Fact]
    public void Execute_PrintStmt_OutputsCorrectEvaluatedExpression()
    {
        var reporter = Substitute.For<IReporter>();
        var interpreter = new TreeWalkInterpreter(reporter);
        var stmt = new PrintStmt(LiteralExpr.Number(67));

        interpreter.Execute(stmt);
        
        reporter.Received().Output(Arg.Is("67"));
    }

    [Fact]
    public void Execute_BlockStmt_OutputsInnerStatements()
    {
        var reporter = Substitute.For<IReporter>();
        var interpreter = new TreeWalkInterpreter(reporter);
        var stmt = new BlockStmt([new PrintStmt(LiteralExpr.Number(1)), new PrintStmt(LiteralExpr.Number(2))]);
        
        interpreter.Execute(stmt);
        
        reporter.Received(1).Output(Arg.Is("1"));
        reporter.Received(1).Output(Arg.Is("2"));
    }

    [Fact]
    public void Execute_IfStmtConditionTrue_OutputsThenBranch()
    {
        var reporter = Substitute.For<IReporter>();
        var interpreter = new TreeWalkInterpreter(reporter);
        var stmt = new IfStmt(LiteralExpr.True(), new PrintStmt(LiteralExpr.Number(67)));
        
        interpreter.Execute(stmt);
        
        reporter.Received(1).Output(Arg.Is("67"));
    }

    [Fact]
    public void Execute_IfStmtConditionFalse_OutputsElseBranch()
    {
        var reporter = Substitute.For<IReporter>();
        var interpreter = new TreeWalkInterpreter(reporter);
        var stmt = new IfStmt(
            LiteralExpr.False(),
            new PrintStmt(LiteralExpr.Number(67)),
            new PrintStmt(LiteralExpr.Number(69)));
        
        interpreter.Execute(stmt);
        
        reporter.Received(1).Output(Arg.Is("69"));
    }

    [Fact]
    public void Execute_IfStmtNonBoolCondition_ThrowsRuntimeError()
    {
        var stmt = new IfStmt(LiteralExpr.Number(1), new PrintStmt(LiteralExpr.Number(1)));
        var interpreter = new TreeWalkInterpreter();
        
        Assert.Throws<RuntimeError>(() => interpreter.Execute(stmt));
    }

    [Fact]
    public void Execute_WhileStmtNonBoolCondition_ThrowsRuntimeError()
    {
        var stmt = new WhileStmt(LiteralExpr.Number(1), new PrintStmt(LiteralExpr.Number(1)));
        var interpreter = new TreeWalkInterpreter();
        
        Assert.Throws<RuntimeError>(() => interpreter.Execute(stmt));
    }

    #endregion
}