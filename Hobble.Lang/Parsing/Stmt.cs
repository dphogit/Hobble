using Hobble.Lang.Lexical;

namespace Hobble.Lang.Parsing;

public abstract record Stmt;

public sealed record ExprStmt(Expr Expr) : Stmt;

public sealed record PrintStmt(Expr Expr) : Stmt;

public sealed record VarStmt(Token Identifier, Expr? Initializer = null) : Stmt;