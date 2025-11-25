using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequireActivation;

public sealed record RequireActivationResponse(string Message) : IResponse;