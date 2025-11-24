using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Presenters.Mailers.TestEmailSending;

public sealed record TestEmailSendingRequest(string TargetEmail, Guid SenderId,  CancellationToken Ct) : IRequest;