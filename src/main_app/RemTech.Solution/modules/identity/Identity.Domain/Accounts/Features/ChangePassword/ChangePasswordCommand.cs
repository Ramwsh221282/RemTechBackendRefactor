using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Domain.Accounts.Features.ChangePassword;

/// <summary>
/// Команда для изменения пароля пользователя.
/// </summary>
/// <param name="AccessToken">Токен доступа.</param>
/// <param name="RefreshToken">Токен обновления.</param>
/// <param name="Id">Идентификатор пользователя.</param>
/// <param name="NewPassword">Новый пароль пользователя.</param>
/// <param name="CurrentPassword">Текущий пароль пользователя.</param>
public sealed record ChangePasswordCommand(
	string AccessToken,
	string RefreshToken,
	Guid Id,
	string NewPassword,
	string CurrentPassword
) : ICommand;
