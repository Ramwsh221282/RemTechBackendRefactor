using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record CreateEmailFunctionArgument(string Input) : IFunctionArgument;

public sealed class CreateEmailFunction : IFunction<CreateEmailFunctionArgument, Result<Email>>
{
    private readonly IFunction<ValidateEmailStringArgument, Result<Unit>> _validateEmail;

    public CreateEmailFunction(IFunction<ValidateEmailStringArgument, Result<Unit>> validateEmail) =>
        _validateEmail = validateEmail;

    public Result<Email> Invoke(CreateEmailFunctionArgument argument) =>
        _validateEmail
            .Invoke(new ValidateEmailStringArgument(argument.Input))
            .Continue(() => Success(new Email(argument.Input)));
}