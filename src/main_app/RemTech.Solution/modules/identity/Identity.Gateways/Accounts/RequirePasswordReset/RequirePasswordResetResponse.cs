using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Gateways.Accounts.RequirePasswordReset;

public sealed record RequirePasswordResetResponse(string Message) : IResponse;