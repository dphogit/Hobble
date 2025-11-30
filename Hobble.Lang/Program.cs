using Hobble.Lang;

Console.WriteLine("Welcome to Hobble v0.1 (Alpha)");
Repl();

return;

void Repl()
{
    var driver = new Driver();
    
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