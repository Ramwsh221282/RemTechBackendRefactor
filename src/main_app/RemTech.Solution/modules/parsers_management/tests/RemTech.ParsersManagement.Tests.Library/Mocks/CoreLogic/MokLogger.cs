using RemTech.Logging.Library;

namespace RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

public sealed class MokLogger : ICustomLogger
{
    public void Info(string template, params object[] arguments) =>
        WriteColoredTemplate(template, arguments, ConsoleColor.White, ConsoleColor.Cyan);

    public void Warn(string template, params object[] arguments) =>
        WriteColoredTemplate(template, arguments, ConsoleColor.White, ConsoleColor.Yellow);

    public void Error(string template, params object[] arguments) =>
        WriteColoredTemplate(template, arguments, ConsoleColor.White, ConsoleColor.Red);

    public void Fatal(string template, params object[] arguments) =>
        WriteColoredTemplate(template, arguments, ConsoleColor.White, ConsoleColor.DarkRed);

    public void WriteColoredTemplate(
        string template,
        object[] arguments,
        ConsoleColor textColor,
        ConsoleColor paramColor
    )
    {
        Console.ForegroundColor = textColor;

        var parts = template.Split('{', '}');

        for (int i = 0; i < parts.Length; i++)
        {
            if (i % 2 == 0)
            {
                Console.Write(parts[i]);
            }
            else
            {
                if (int.TryParse(parts[i], out int paramIndex) && paramIndex < arguments.Length)
                {
                    Console.ForegroundColor = paramColor;
                    Console.Write(arguments[paramIndex]);
                    Console.ForegroundColor = textColor;
                }
                else
                {
                    Console.Write($"{{{parts[i]}}}");
                }
            }
        }

        Console.WriteLine();
        Console.ResetColor();
    }
}
