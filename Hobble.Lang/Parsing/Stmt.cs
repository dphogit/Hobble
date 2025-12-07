using Hobble.Lang.Lexical;

namespace Hobble.Lang.Parsing;

public abstract record Stmt;

public sealed record BlockStmt(IList<Stmt> Stmts) : Stmt
{
    public bool Equals(BlockStmt? other)
    {
        return other is not null && Stmts.SequenceEqual(other.Stmts);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), Stmts);
    }
}

public sealed record ExprStmt(Expr Expr) : Stmt;

public sealed record IfStmt(Expr Condition, Stmt Then, Stmt? Else = null) : Stmt;

public sealed record PrintStmt(Expr Expr) : Stmt;

public sealed record VarStmt(Token Identifier, Expr? Initializer = null) : Stmt;

public sealed record WhileStmt(Expr Condition, Stmt Body) : Stmt;