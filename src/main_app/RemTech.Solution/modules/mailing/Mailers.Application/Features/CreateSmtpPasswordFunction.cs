using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record CreateSmtpPasswordArgument(string Value) : IFunctionArgument;

public sealed class CreateSmtpPasswordFunction : IFunction<CreateSmtpPasswordArgument, Result<SmtpPassword>>
{
    private readonly IFunction<ValidateSmtpPasswordFunctionArgument, Result<Unit>> _validate;

    public CreateSmtpPasswordFunction(IFunction<ValidateSmtpPasswordFunctionArgument, Result<Unit>> validate) =>
        _validate = validate;

    public Result<SmtpPassword> Invoke(CreateSmtpPasswordArgument argument)
    {
        var validation = _validate.Invoke(new ValidateSmtpPasswordFunctionArgument(argument.Value));
        return validation.IsFailure ? validation.Error : new SmtpPassword(argument.Value);
    }
}