using Hobble.Lang.Lexical;

namespace Hobble.Lang.Interface;

public class ConsoleReporter : IReporter
{
    public void Error(Token token, string message)
    {
        Console.Error.WriteLine($"[Line {token.Line}] Error: {message}");
    }

    public void Error(string message)
    {
        Console.Error.WriteLine(message);
    }

    public void Output(string message)
    {
        Console.WriteLine(message);
    }
}