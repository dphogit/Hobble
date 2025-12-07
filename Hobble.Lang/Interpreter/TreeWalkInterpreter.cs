using Hobble.Lang.Interface;
using Hobble.Lang.Lexical;
using Hobble.Lang.Parsing;
using Hobble.Lang.Representation;

namespace Hobble.Lang.Interpreter;

public class TreeWalkInterpreter(IReporter reporter)
{
    private VariableEnvironment _environment = new();
    
    public TreeWalkInterpreter() : this(new ConsoleReporter()) { }

    public bool HadRuntimeError;

    public void Interpret(ParseTree parseTree)
    {
        try
        {
            foreach (var stmt in parseTree.Stmts)
                Execute(stmt);
        }
        catch (RuntimeError error)
        {
            reporter.Error($"Runtime Error: {error.Message}");
            HadRuntimeError = true;
        }
    }
    
    #region Statement Execution

    public void Execute(Stmt stmt)
    {
        switch (stmt)
        {
            case BlockStmt blockStmt:
                ExecuteBlockStmt(blockStmt, new VariableEnvironment(_environment));
                return;
            case ExprStmt exprStmt:
                Evaluate(exprStmt.Expr);
                return;
            case PrintStmt printStmt:
                ExecutePrintStmt(printStmt);
                return;
            case VarStmt varStmt:
                ExecuteVarStmt(varStmt);
                return;
            default:
                throw new ArgumentException($"Invalid statement type {stmt.GetType()}");
        }
    }

    private void ExecuteBlockStmt(BlockStmt blockStmt, VariableEnvironment environment)
    {
        // Keep reference to previous environment because it needs to be restored
        // after executing the block statement with the passed environment.
        var previousEnvironment = _environment;

        try
        {
            _environment = environment;
            
            foreach (var stmt in blockStmt.Stmts)
                Execute(stmt);
        }
        finally
        {
            _environment = previousEnvironment;
        }
    }

    private void ExecutePrintStmt(PrintStmt printStmt)
    {
        var result = Evaluate(printStmt.Expr);
        reporter.Output(result.ToString());
    }

    private void ExecuteVarStmt(VarStmt varStmt)
    {
        var value = varStmt.Initializer is null ? HobbleValue.Null() : Evaluate(varStmt.Initializer);
        _environment.Define(varStmt.Identifier.Lexeme, value);
    }
    
    #endregion
    
    #region Expression Evaluation 
    
    public HobbleValue Evaluate(Expr expression)
    {
        return expression switch
        {
            AssignExpr assignExpr => EvaluateAssignExpr(assignExpr),
            BinaryExpr binaryExpr => EvaluateBinaryExpr(binaryExpr),
            GroupExpr groupExpr => Evaluate(groupExpr.Expr),
            LiteralExpr literalExpr => literalExpr.Value,
            UnaryExpr unaryExpr => EvaluateUnaryExpr(unaryExpr),
            VarExpr varExpr => EvaluateVarExpr(varExpr),
            _ => throw new ArgumentException($"Invalid expression type {expression.GetType()}")
        };
    }

    private HobbleValue EvaluateAssignExpr(AssignExpr assignExpr)
    {
        var value = Evaluate(assignExpr.Value);
        _environment.Assign(assignExpr.Identifier.Lexeme, value);
        return value;
    }
    
    private HobbleValue EvaluateBinaryExpr(BinaryExpr binaryExpr)
    {
        var op = binaryExpr.Operator;

        // Some operators short-circuit so we only want to evaluate what is needed,
        // hence we don't evaluate and assign immediately.
        HobbleValue left, right;

        if (op.Type == TokenType.AmpAmp)
        {
            left = Evaluate(binaryExpr.Left);
            CheckBoolOperand(left);

            // If A is false, then A && B is false.
            if (!left.AsBool())
                return HobbleValue.Bool(false);
            
            right = Evaluate(binaryExpr.Right);
            CheckBoolOperand(right);
            return right;
        }

        if (op.Type == TokenType.PipePipe)
        {
            left = Evaluate(binaryExpr.Left);
            CheckBoolOperand(left);

            // If A is true, then A || B is true.
            if (left.AsBool())
                return HobbleValue.Bool(true);
            
            right = Evaluate(binaryExpr.Right);
            CheckBoolOperand(right);
            return right;
        }
        
        left = Evaluate(binaryExpr.Left);
        right = Evaluate(binaryExpr.Right);
        
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
            case TokenType.LessThan:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() < right.AsNumber());
            }
            case TokenType.GreaterThan:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() > right.AsNumber());
            }
            case TokenType.LessThanEqual:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() <= right.AsNumber());
            }
            case TokenType.GreaterThanEqual:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() >= right.AsNumber());
            }
            case TokenType.EqualEqual:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() == right.AsNumber());
            }
            case TokenType.BangEqual:
            {
                CheckNumberOperands(left, right);
                return HobbleValue.Bool(left.AsNumber() != right.AsNumber());
            }
            default:
                throw new ArgumentException($"Invalid binary expression type '{binaryExpr.GetType()}'");
        }

        void CheckNumberOperands(HobbleValue a, HobbleValue b)
        {
            if (!a.IsNumber() || !b.IsNumber())
                throw new RuntimeError("Operands must both be Numbers.");
        }

        void CheckBoolOperand(HobbleValue a)
        {
            if (!a.IsBool())
                throw new RuntimeError("Operand must be Bool.");
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
            
            TokenType.Bang => right.IsBool()
                ? HobbleValue.Bool(!right.AsBool())
                : throw new RuntimeError("Logical negation operand must be a Bool."),
            
            _ => throw new ArgumentException($"Invalid unary expression type '{unaryExpr.GetType()}'")
        };
    }

    private HobbleValue EvaluateVarExpr(VarExpr varExpr)
    {
        return _environment.Get(varExpr.Identifier.Lexeme);
    }
    
    #endregion
}