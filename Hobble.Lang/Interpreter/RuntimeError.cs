namespace Hobble.Lang.Interpreter;

public class RuntimeError : Exception
{
    public RuntimeError(string message) : base(message) { }

    public RuntimeError(Exception exception) : base(exception.Message, exception) { }
}

public class DivideByZeroError() : RuntimeError("Division by zero.") { }