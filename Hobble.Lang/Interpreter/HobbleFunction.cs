using Hobble.Lang.Parsing;

namespace Hobble.Lang.Interpreter;

public class HobbleFunction(FnStmt declaration)
{
    public readonly string Name = declaration.Identifier.Lexeme;
    public readonly int Arity = declaration.Parameters.Count;
    
    public HobbleValue Call(
        IList<HobbleValue> arguments,
        Action<BlockStmt, VariableEnvironment> executeBody,
        VariableEnvironment? parentEnvironment = null)
    {
        // Create own call environment and bind the parameters to the called arguments.
        var environment = new VariableEnvironment(parentEnvironment);
        for (var i = 0; i < declaration.Parameters.Count; i++)
            environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);

        try
        {
            executeBody.Invoke(declaration.Body, environment);
        }
        catch (ReturnValue returnValue)
        {
            return returnValue.Value;
        }
        
        return HobbleValue.Null();
    }

    public override string ToString()
    {
        return $"<fn {Name}>";
    }
}