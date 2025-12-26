using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequirePasswordReset;

public sealed record RequirePasswordResetRequest(Guid Id, CancellationToken Ct) : IRequest;