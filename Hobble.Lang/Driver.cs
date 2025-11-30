using Hobble.Lang.Interpreter;
using Hobble.Lang.Parsing;

namespace Hobble.Lang;

public class Driver
{
    public void Run(string source)
    {
        var parser = new Parser();
        var interpreter = new TreeWalkInterpreter();

        var expression = parser.ParseExpression(source);
        
        var value = interpreter.Evaluate(expression);
        
        Console.WriteLine(value);
    }
}