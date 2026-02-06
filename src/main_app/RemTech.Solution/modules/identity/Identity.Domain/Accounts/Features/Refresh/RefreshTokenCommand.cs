using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Refresh;

/// <summary>
/// Команда для обновления токенов пользователя.
/// </summary>
/// <param name="AccessToken">Токен доступа пользователя.</param>
/// <param name="RefreshToken">Токен обновления пользователя.</param>
public sealed record RefreshTokenCommand(string AccessToken, string RefreshToken) : ICommand;
