using Hobble.Lang.Lexical;
using Hobble.Lang.Representation;

namespace Hobble.Lang.Parsing;

public abstract record Expr;

public sealed record AssignExpr(Token Identifier, Expr Value) : Expr;

public sealed record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;

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