using Hobble.Lang.Lexical;

namespace Hobble.Lang.Interface;

public class ConsoleReporter : IReporter
{
    public void Error(Token token, string message)
    {
        var at = token.Type == TokenType.Eof ? "end" : $"'{token.Lexeme}'";
        ReportError(token.Line, at, message);
    }

    private static void ReportError(int line, string at, string message)
    {
        Console.Error.WriteLine($"[Line {line}] Error at {at}: {message}");
    }
}