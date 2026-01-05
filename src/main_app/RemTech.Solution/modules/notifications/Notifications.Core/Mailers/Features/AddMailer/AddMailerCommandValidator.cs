using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.AddMailer;

public sealed class AddMailerCommandValidator : AbstractValidator<AddMailerCommand>
{
    public AddMailerCommandValidator()
    {
        RuleFor(c => new { c.Email, c.SmtpPassword })
            .MustBeValid(o => MailerCredentials.Create(o.SmtpPassword, o.Email));
    }
}