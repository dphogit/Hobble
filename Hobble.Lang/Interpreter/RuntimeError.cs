namespace Hobble.Lang.Interpreter;

public class RuntimeError(string message) : Exception(message) { }

public class DivideByZeroError() : RuntimeError("Division by zero.") { }