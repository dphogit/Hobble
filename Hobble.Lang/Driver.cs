using Hobble.Lang.Interface;
using Hobble.Lang.Interpreter;
using Hobble.Lang.Parsing;

namespace Hobble.Lang;

public class Driver(IReporter reporter)
{
    private readonly Parser _parser = new(reporter);
    private readonly TreeWalkInterpreter _interpreter = new();
    
    public Driver() : this(new ConsoleReporter()) { }
    
    public void Run(string source)
    {
        var stmt = _parser.ParseStatement(source);
        _interpreter.Execute(stmt);
    }

    public void RunFile(string file)
    {
        throw new NotImplementedException("File running not implemented yet.");
    }
}