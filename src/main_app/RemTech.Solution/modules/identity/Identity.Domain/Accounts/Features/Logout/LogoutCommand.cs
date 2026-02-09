using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Logout;

/// <summary>
/// Команда для выхода пользователя из системы.
/// </summary>
/// <param name="AccessToken">Токен доступа пользователя.</param>
/// <param name="RefreshToken">Токен обновления пользователя.</param>
public sealed record LogoutCommand(string AccessToken, string RefreshToken) : ICommand;
