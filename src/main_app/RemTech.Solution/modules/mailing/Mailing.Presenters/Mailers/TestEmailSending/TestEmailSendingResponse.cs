using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Presenters.Mailers.TestEmailSending;

public sealed record TestEmailSendingResponse(Guid Id) : IResponse;