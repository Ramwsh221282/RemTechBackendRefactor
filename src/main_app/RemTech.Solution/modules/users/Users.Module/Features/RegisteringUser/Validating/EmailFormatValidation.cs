using System.Text.RegularExpressions;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.RegisteringUser.Validating;

internal sealed partial class EmailFormatValidation(string input) : IValidation
{
    private static readonly Regex EmailRegex = MyRegex();

    public void Check()
    {
        if (!EmailRegex.IsMatch(input))
            throw new ArgumentException($"Некорректный формат почты: {input}");
    }

    [GeneratedRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
