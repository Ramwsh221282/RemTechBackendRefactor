using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Core.PendingEmails.Features.AddPendingEmail;

public sealed record AddPendingEmailCommand(string Recipient, string Subject, string Body) : ICommand;
