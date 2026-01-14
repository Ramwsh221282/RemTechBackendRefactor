using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Logout;

public sealed record LogoutCommand(string AccessToken, string RefreshToken) : ICommand;