using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.RegisterAccount;

/// <summary>
/// Команда для регистрации нового аккаунта пользователя.
/// </summary>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Password">Пароль пользователя.</param>
public sealed record RegisterAccountCommand(string Email, string Login, string Password) : ICommand;
