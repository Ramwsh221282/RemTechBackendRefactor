using FluentValidation;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.Mailers.Features.DeleteMailer;

public sealed class DeleteMailerValidator : AbstractValidator<DeleteMailerCommand>
{
    public DeleteMailerValidator()
    {
        RuleFor(x => x.Id).MustBeValid(MailerId.Create);
    }
}