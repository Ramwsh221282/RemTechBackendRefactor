using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.VerifyToken;

public sealed record VerifyTokenCommand(string Token) : ICommand;
