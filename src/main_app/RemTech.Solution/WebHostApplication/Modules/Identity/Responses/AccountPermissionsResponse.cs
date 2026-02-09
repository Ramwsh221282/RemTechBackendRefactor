using Identity.Domain.Permissions;

namespace WebHostApplication.Modules.Identity.Responses;

/// <summary>
/// Ответ с информацией о праве пользователя.
/// </summary>
/// <param name="Id">Идентификатор права.</param>
/// <param name="Name">Название права.</param>
/// <param name="Description">Описание права.</param>
public sealed record AccountPermissionsResponse(Guid Id, string Name, string Description)
{
	/// <summary>
	/// Преобразование из доменной модели права в ответ.
	/// </summary>
	/// <param name="permission">Доменная модель права.</param>
	/// <returns>Ответ с информацией о праве пользователя.</returns>
	public static AccountPermissionsResponse ConvertFrom(Permission permission)
	{
		return new(permission.Id.Value, permission.Name.Value, permission.Description.Value);
	}
}
