using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequireActivation;

public sealed record RequireActivationRequest(Guid Id, CancellationToken Ct) : IRequest;