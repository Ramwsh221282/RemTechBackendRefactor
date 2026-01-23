using FluentValidation;
using RemTech.SharedKernel.Core.Handlers.Decorators.Validation;

namespace Notifications.Core.Mailers.Features.ChangeCredentials;

public sealed class ChangeCredentialsValidator : AbstractValidator<ChangeCredentialsCommand>
{
    public ChangeCredentialsValidator()
    {
        RuleFor(c => c.Id).MustBeValid(MailerId.Create);
        RuleFor(c => new { c.SmtpPassword, c.Email })
            .MustBeValid(o => MailerCredentials.Create(o.SmtpPassword, o.Email));
    }
}
