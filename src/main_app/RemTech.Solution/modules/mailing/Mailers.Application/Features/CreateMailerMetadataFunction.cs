using Mailers.Core.MailersContext;

namespace Mailers.Application.Features;

public sealed record CreateMailerMetadataArguments(string Email, string Password, Guid? Id = null) : IFunctionArgument;

public sealed class CreateMailerMetadataFunction : IFunction<CreateMailerMetadataArguments, Result<MailerMetadata>>
{
    private readonly IFunction<CreateEmailFunctionArgument, Result<Email>> _createEmail;
    private readonly IFunction<CreateSmtpPasswordArgument, Result<SmtpPassword>> _createPassword;

    public CreateMailerMetadataFunction(
        IFunction<CreateEmailFunctionArgument, Result<Email>> createEmail,
        IFunction<CreateSmtpPasswordArgument, Result<SmtpPassword>> createPassword) =>
        (_createEmail, _createPassword) = (createEmail, createPassword);

    public Result<MailerMetadata> Invoke(CreateMailerMetadataArguments argument)
    {
        var id = ResolveId(argument.Id);
        var email = _createEmail.Invoke(new CreateEmailFunctionArgument(argument.Email));
        var password = _createPassword.Invoke(new CreateSmtpPasswordArgument(argument.Password));
        if (id.IsFailure) return id.Error;
        if (email.IsFailure) return email.Error;
        if (password.IsFailure) return password.Error;
        return new MailerMetadata(id, email, password);
    }

    private Result<Guid> ResolveId(Guid? id) =>
        id != null
            ? id == Guid.Empty ? Validation("Идентификатор пустой.") : Success(id.Value)
            : Success(Guid.NewGuid());
}