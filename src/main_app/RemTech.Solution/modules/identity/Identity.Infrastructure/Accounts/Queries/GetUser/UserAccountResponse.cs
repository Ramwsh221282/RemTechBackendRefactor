using Identity.Domain.Accounts.Models;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

/// <summary>
/// Ответ с информацией об аккаунте пользователя.
/// </summary>
/// <param name="Id">Идентификатор аккаунта пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="IsActivated">Статус активации аккаунта.</param>
/// <param name="Permissions">Список разрешений пользователя.</param>
public sealed record UserAccountResponse(
	Guid Id,
	string Login,
	string Email,
	bool IsActivated,
	IEnumerable<UserAccountPermissionResponse> Permissions
)
{
	/// <summary>
	/// Создает экземпляр <see cref="UserAccountResponse"/> из модели <see cref="Account"/>.
	/// </summary>
	/// <param name="account">Модель аккаунта пользователя.</param>
	/// <returns>Экземпляр <see cref="UserAccountResponse"/>.</returns>
	public static UserAccountResponse Create(Account account) =>
		new(
			account.Id.Value,
			account.Login.Value,
			account.Email.Value,
			account.ActivationStatus.Value,
			account.PermissionsList.Select(p => UserAccountPermissionResponse.Create(p))
		);
}
