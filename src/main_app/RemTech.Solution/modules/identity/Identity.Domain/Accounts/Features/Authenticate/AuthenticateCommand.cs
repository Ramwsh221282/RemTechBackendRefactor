using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Authenticate;

/// <summary>
/// Команда для аутентификации пользователя.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
public sealed record AuthenticateCommand(string? Login, string? Email, string Password) : ICommand;
