using Hobble.Lang.Interpreter;
using Hobble.Lang.Lexical;

namespace Hobble.Lang.Parsing;

public abstract record Expr;

public sealed record AssignExpr(Token Identifier, Expr Value) : Expr;

public sealed record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;

public sealed record CallExpr(Expr Callee, IList<Expr> Arguments) : Expr
{
    public CallExpr(Expr callee) : this(callee, []) { }

    public bool Equals(CallExpr? other)
    {
        return other is not null &&
               Callee == other.Callee &&
               Arguments.SequenceEqual(other.Arguments);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Callee, Arguments);
    }
}

public sealed record GroupExpr(Expr Expr) : Expr;

public sealed record LiteralExpr(HobbleValue Value) : Expr
{
    public static LiteralExpr Number(decimal value) => new(HobbleValue.Number(value));
    public static LiteralExpr String(string value) => new(HobbleValue.String(value));
    public static LiteralExpr Bool(bool value) => new(HobbleValue.Bool(value));
    public static LiteralExpr True() => new(HobbleValue.Bool(true));
    public static LiteralExpr False() => new(HobbleValue.Bool(false));
}

public sealed record UnaryExpr(Token Operator, Expr Right) : Expr;

public sealed record VarExpr(Token Identifier) : Expr;