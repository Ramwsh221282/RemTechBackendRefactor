namespace Mailers.Application.Features;

public sealed record ValidateSmtpPasswordFunctionArgument(string? Input) : IFunctionArgument;

public sealed class ValidateSmtpPasswordStringFunction : IFunction<ValidateSmtpPasswordFunctionArgument, Result<Unit>>
{
    public Result<Unit> Invoke(ValidateSmtpPasswordFunctionArgument argument)
    {
        var password = argument.Input;
        return Invariant
            .For(password, Strings.NotEmptyOrWhiteSpace)
            .BindError(Validation("SMTP пароль не указан."))
            .Map(() => Unit.Value);
    }
}