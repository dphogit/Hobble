using Hobble.Lang.Representation;

namespace Hobble.Lang.Interpreter;

public class VariableEnvironment
{
    private readonly Dictionary<string, HobbleValue> _variables = new();

    /// <summary>Define a variable with the associated value.</summary>
    /// <param name="name">The variable identifier.</param>
    /// <param name="value">The value to assign to the variable.</param>
    /// <exception cref="VariableAlreadyDefinedError">The variable name is already defined.</exception>
    /// <remarks>Variables cannot be redefined in the same variable environment scope.</remarks>
    public void Define(string name, HobbleValue value)
    {
        if (!_variables.TryAdd(name, value))
            throw new VariableAlreadyDefinedError(name);
    }

    /// <summary>Gets the value associated with the specified variable name.</summary>
    /// <param name="name">The name of the variable to retrieve.</param>
    /// <returns>The value associated with the variable.</returns>
    /// <exception cref="UndefinedVariableError">The specified variable has not been defined.</exception>
    public HobbleValue Get(string name)
    {
        if (_variables.TryGetValue(name, out var value))
            return value;

        throw new UndefinedVariableError(name);
    }

    public void Assign(string name, HobbleValue value)
    {
        if (!_variables.ContainsKey(name))
            throw new UndefinedVariableError(name);
        
        _variables[name]  = value;
    }
}

public class VariableAlreadyDefinedError(string name) : RuntimeError($"Variable '{name}' is already defined.");
public class UndefinedVariableError(string name) : RuntimeError($"Undefined variable '{name}'.");