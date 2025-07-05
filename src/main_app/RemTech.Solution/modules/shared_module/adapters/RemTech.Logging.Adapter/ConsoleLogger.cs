using RemTech.Logging.Library;

namespace RemTech.Logging.Adapter;

public sealed class ConsoleLogger : ICustomLogger
{
    public void Info(string template, params object[] arguments)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(template, arguments);
        Console.ResetColor();
    }

    public void Warn(string template, params object[] arguments)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(template, arguments);
        Console.ResetColor();
    }

    public void Error(string template, params object[] arguments)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(template, arguments);
        Console.ResetColor();
    }

    public void Fatal(string template, params object[] arguments)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(template, arguments);
        Console.ResetColor();
    }
}
