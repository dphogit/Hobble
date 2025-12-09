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

    public bool RunFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            reporter.Error($"File '{filePath}' not found.");
            return false;
        }

        if (Path.GetExtension(filePath) != ".hob")
        {
            reporter.Error("Not a .hob file.");
            return false;
        }

        try
        {
            var source = File.ReadAllText(filePath);
            return Run(source);
        }
        catch (Exception e)
        {
            reporter.Error($"Failed to read file: {e.Message}");
            return false;
        }
    }
}