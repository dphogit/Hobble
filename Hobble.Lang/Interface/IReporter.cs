using Hobble.Lang.Lexical;

namespace Hobble.Lang.Interface;

public interface IReporter
{
    void Error(Token token, string message);

    void Error(string message);

    void Output(string message);
}