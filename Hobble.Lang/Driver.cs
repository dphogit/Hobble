using Hobble.Lang.Interface;
using Hobble.Lang.Interpreter;
using Hobble.Lang.Parsing;

namespace Hobble.Lang;

public class Driver(IReporter reporter)
{
    private readonly Parser _parser = new(reporter);
    private readonly TreeWalkInterpreter _interpreter = new(reporter);
    
    public Driver() : this(new ConsoleReporter()) { }
    
    /// <summary>Runs the given program source.</summary>
    /// <param name="source">The source code.</param>
    /// <returns>True if the source was run successfully.</returns>
    public bool Run(string source)
    {
        var parseTree = _parser.ParseProgram(source);

        if (_parser.HadError)
            return false;

        _interpreter.Interpret(parseTree);
        return !_interpreter.HadRuntimeError;
    }

    public void RunFile(string file)
    {
        throw new NotImplementedException("File running not implemented yet.");
    }
}