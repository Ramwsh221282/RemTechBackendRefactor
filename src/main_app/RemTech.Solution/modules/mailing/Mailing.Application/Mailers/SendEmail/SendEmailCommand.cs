using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Application.Mailers.SendEmail;

public sealed record SendEmailCommand(
    string TargetEmail, 
    string Subject, 
    string Body, 
    CancellationToken Ct,
    string? SenderEmail = null, 
    Guid? SenderId = null) : ICommand;