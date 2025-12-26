using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Application.Mailers.UpdateMailer;

public record UpdateMailerCommand(
    Guid Id, 
    string NewEmail, 
    string NewPassword, 
    CancellationToken Ct) : ICommand;