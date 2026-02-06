using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ResetPassword;

/// <summary>
/// Команда для сброса пароля пользователя.
/// </summary>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
public sealed record ResetPasswordCommand(string? Login, string? Email) : ICommand;
