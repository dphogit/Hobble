using Hobble.Lang;
using Hobble.Lang.Interface;

var driver = new Driver(new ConsoleReporter());

switch (args.Length)
{
    case 0:
    {
        Repl();
        break;
    }
    case 1:
    {
        var fileName = args[0];
        driver.RunFile(fileName);
        break;
    }
    default:
        Console.WriteLine("Usage: hobble [FILE]");
        break;
}

return;

void Repl()
{
    Console.WriteLine("Welcome to Hobble v0.1 (Alpha)");
    
    while (true)
    {
        Console.Write("> ");

        var source = Console.ReadLine();

        if (source is null)
        {
            Console.Error.WriteLine("Failed to read source.");
            continue;
        }

        driver.Run(source);
    }
    
    // ReSharper disable once FunctionNeverReturns
}