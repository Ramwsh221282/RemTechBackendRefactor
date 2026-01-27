using Identity.Domain.Permissions;

namespace Identity.Infrastructure.Accounts.Queries.GetUser;

/// <summary>
/// Ответ с информацией о разрешении аккаунта пользователя.
/// </summary>
/// <param name="Id">Идентификатор разрешения.</param>
/// <param name="Name">Название разрешения.</param>
/// <param name="Description">Описание разрешения.</param>
public sealed record UserAccountPermissionResponse(Guid Id, string Name, string Description)
{
	/// <summary>
	/// Создает экземпляр <see cref="UserAccountPermissionResponse"/> из модели <see cref="Permission"/>.
	/// </summary>
	/// <param name="permission">Модель разрешения.</param>
	/// <returns>Экземпляр <see cref="UserAccountPermissionResponse"/>.</returns>
	public static UserAccountPermissionResponse Create(Permission permission) =>
		new(permission.Id.Value, permission.Name.Value, permission.Description.Value);
}
