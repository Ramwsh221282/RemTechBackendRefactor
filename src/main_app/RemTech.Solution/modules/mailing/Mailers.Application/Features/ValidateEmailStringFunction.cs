using System.Text.RegularExpressions;

namespace Mailers.Application.Features;

public sealed record ValidateEmailStringArgument(string? Input) : IFunctionArgument;

public sealed class ValidateEmailStringFunction : IFunction<ValidateEmailStringArgument, Result<Unit>>
{
    public Result<Unit> Invoke(ValidateEmailStringArgument argument)
    {
        var res = Invariant
            .For(argument.Input, Strings.NotEmptyOrWhiteSpace).BindError(Validation("Почта не указана"))
            .SwitchTo(argument.Input, i => Strings.NotGreaterThan(i!, 256)).BindError(Validation("Почта некорректная"))
            .SwitchTo(argument.Input, MatchesEmailRegex).BindError(Validation("Почта некорректная"))
            .Map(() => Unit.Value);
        return res;
    }

    private static bool MatchesEmailRegex(string? email)
    {
        bool matches = !string.IsNullOrWhiteSpace(email) && Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return matches;
    }
}