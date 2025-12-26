namespace Mailers.Application.Features.SendEmailMessage;

public sealed record SendEmailMessageArgs(Guid Id, string To, string Subject, string Body, CancellationToken Ct);