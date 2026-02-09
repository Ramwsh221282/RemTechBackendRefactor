using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Dev_ChangeEmail;

/// <summary>
/// Команда для изменения email пользователя в режиме разработки.
/// </summary>
/// <param name="NewEmail">Новый email пользователя.</param>
/// <param name="AccountId">Идентификатор аккаунта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Текущий email пользователя.</param>
public sealed record Dev_ChangeEmailCommand(
	string NewEmail,
	Guid? AccountId = null,
	string? Login = null,
	string? Email = null
) : ICommand;
