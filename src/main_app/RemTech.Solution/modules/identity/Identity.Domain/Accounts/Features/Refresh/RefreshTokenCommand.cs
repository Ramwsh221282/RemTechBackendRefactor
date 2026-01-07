using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Refresh;

public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand;