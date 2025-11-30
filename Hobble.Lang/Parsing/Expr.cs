using Hobble.Lang.Lexical;
using Hobble.Lang.Representation;

namespace Hobble.Lang.Parsing;

public abstract record Expr;

public record BinaryExpr(Expr Left, Token Operator, Expr Right) : Expr;

public record LiteralExpr(HobbleValue Value) : Expr
{
    public static LiteralExpr Number(decimal value) => new (HobbleValue.Number(value));
    public static LiteralExpr String(string value) => new (HobbleValue.String(value));
}