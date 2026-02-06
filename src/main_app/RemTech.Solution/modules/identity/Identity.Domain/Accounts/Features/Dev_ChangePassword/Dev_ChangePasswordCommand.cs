using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.Dev_ChangePassword;

/// <summary>
/// Команда для изменения пароля пользователя в режиме разработки.
/// </summary>
/// <param name="NewPassword">Новый пароль пользователя.</param>
/// <param name="AccountId">Идентификатор аккаунта пользователя.</param>
/// <param name="AccountLogin">Логин пользователя.</param>
/// <param name="AccountEmail">Email пользователя.</param>
public sealed record Dev_ChangePasswordCommand(
	string NewPassword,
	Guid? AccountId = null,
	string? AccountLogin = null,
	string? AccountEmail = null
) : ICommand;
