using Identity.Domain.Accounts.Models;

namespace WebHostApplication.Modules.identity.Responses;

/// <summary>
/// Ответ с информацией об учетной записи пользователя.
/// </summary>
/// <param name="Id">Идентификатор учетной записи пользователя.</param>
/// <param name="Login">Логин пользователя.</param>
/// <param name="Email">Электронная почта пользователя.</param>
/// <param name="IsActivated">Статус активации учетной записи.</param>
/// <param name="Permissions">Список прав пользователя.</param>
public sealed record AccountResponse(
	Guid Id,
	string Login,
	string Email,
	bool IsActivated,
	IEnumerable<AccountPermissionsResponse> Permissions
)
{
	/// <summary>
	/// Преобразование из доменной модели учетной записи в ответ.
	/// </summary>
	/// <param name="account">Доменная модель учетной записи.</param>
	/// <returns>Ответ с информацией об учетной записи пользователя.</returns>
	public static AccountResponse ConvertFrom(Account account)
	{
		return new(
			account.Id.Value,
			account.Login.Value,
			account.Email.Value,
			account.ActivationStatus.Value,
			account.PermissionsList.Select(AccountPermissionsResponse.ConvertFrom)
		);
	}
}
