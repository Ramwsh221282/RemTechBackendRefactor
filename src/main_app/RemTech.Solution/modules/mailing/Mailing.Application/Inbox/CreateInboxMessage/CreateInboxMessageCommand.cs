using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Application.Inbox.CreateInboxMessage;

public sealed record CreateInboxMessageCommand(
    string TargetEmail, 
    string Subject, 
    string Body,
    CancellationToken Ct = default) : ICommand;