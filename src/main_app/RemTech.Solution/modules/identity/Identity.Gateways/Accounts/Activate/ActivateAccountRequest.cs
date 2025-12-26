using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.Activate;

public sealed record ActivateAccountRequest(Guid Id, CancellationToken Ct) : IRequest;