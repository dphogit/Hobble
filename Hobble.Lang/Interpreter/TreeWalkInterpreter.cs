using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;
using Hobble.Lang.Representation;

namespace Hobble.Lang.Interpreter;

public class TreeWalkInterpreter
{
    public HobbleValue Evaluate(Expr expression)
    {
        return expression switch
        {
            BinaryExpr binaryExpr => EvaluateBinaryExpr(binaryExpr),
            LiteralExpr literalExpr => literalExpr.Value,
            UnaryExpr unaryExpr => EvaluateUnaryExpr(unaryExpr),
            _ => throw new ArgumentException($"Invalid expression type {expression.GetType()}")
        };
    }

    private HobbleValue EvaluateBinaryExpr(BinaryExpr binaryExpr)
    {
        var left = Evaluate(binaryExpr.Left);
        var right = Evaluate(binaryExpr.Right);
        
        var op = binaryExpr.Operator;

        switch (op.Type)
        {
            case TokenType.Plus:
            {
                if (left.IsNumber() && right.IsNumber())
                    return HobbleValue.Number(left.AsNumber() + right.AsNumber());
                if (left.IsString() && right.IsString())
                    return HobbleValue.String(left.AsString() + right.AsString());
                throw new RuntimeError("Operand types must both be Numbers or both be Strings.");
            }
            case TokenType.Minus:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Number(left.AsNumber() - right.AsNumber());
            }
            case TokenType.Star:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Number(left.AsNumber() * right.AsNumber());
            }
            case TokenType.Slash:
            {
                CheckNumberOperands(left, right);
                var rightNumber = right.AsNumber();
                return rightNumber == 0
                    ? throw new DivideByZeroError()
                    : HobbleValue.Number(left.AsNumber() / right.AsNumber());
            }
            default:
                throw new ArgumentException($"Invalid binary expression type '{binaryExpr.GetType()}'");
        }

        void CheckNumberOperands(HobbleValue a, HobbleValue b)
        {
            if (!a.IsNumber() || !b.IsNumber())
                throw new RuntimeError("Operands must both be Numbers.");
        }
    }

    private HobbleValue EvaluateUnaryExpr(UnaryExpr unaryExpr)
    {
        var op = unaryExpr.Operator;
        var right = Evaluate(unaryExpr.Right);

        return op.Type switch
        {
            TokenType.Minus => right.IsNumber()
                ? HobbleValue.Number(-right.AsNumber())
                : throw new RuntimeError("Negation operand must be a Number."),
            
            _ => throw new ArgumentException($"Invalid unary expression type '{unaryExpr.GetType()}'")
        };
    }
}