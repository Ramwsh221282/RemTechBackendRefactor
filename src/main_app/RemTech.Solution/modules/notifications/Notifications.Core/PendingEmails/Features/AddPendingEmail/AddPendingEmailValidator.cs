using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

public sealed class AddPendingEmailValidator : AbstractValidator<AddPendingEmailCommand>
{
    public AddPendingEmailValidator()
    {
        RuleFor(x => new { x.Recipient, x.Subject, x.Body })
            .MustBeValid(o => PendingEmailNotification.CreateNew(o.Recipient, o.Subject, o.Body));
    }
}