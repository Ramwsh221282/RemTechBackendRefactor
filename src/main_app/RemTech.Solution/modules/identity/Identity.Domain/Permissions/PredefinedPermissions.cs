namespace Identity.Domain.Permissions;

/// <summary>
/// Предопределенные разрешения.
/// </summary>
public static class PredefinedPermissions
{
	/// <summary>
	/// Разрешение на управление уведомлениями.
	/// </summary>
	public static readonly PermissionName NotificationsManagementName = PermissionName.Create(
		"notifications.management"
	);

	/// <summary>
	/// Описание разрешения на управление уведомлениями.
	/// </summary>
	public static readonly PermissionName NotificationsManagementDescription = PermissionName.Create(
		"Управление рассылкой уведомлений."
	);

	/// <summary>
	/// Разрешение на управление учетными записями.
	/// </summary>
	public static readonly PermissionName IdentityManagementName = PermissionName.Create("identity.management");

	/// <summary>
	/// Описание разрешения на управление учетными записями.
	/// </summary>
	public static readonly PermissionName IdentityManagementDescription = PermissionName.Create(
		"Управление учетными записями."
	);

	/// <summary>
	/// Разрешение на управление парсерами.
	/// </summary>
	public static readonly PermissionName ParserManagementName = PermissionName.Create("parser.management");

	/// <summary>
	/// Описание разрешения на управление парсерами.
	/// </summary>
	public static readonly PermissionName ParserManagementDescription = PermissionName.Create("Управление парсерами.");

	/// <summary>
	/// Разрешение на просмотр источников данных.
	/// </summary>
	public static readonly PermissionName WatchItemSourcesName = PermissionName.Create("watch.item.sources");

	/// <summary>
	/// Описание разрешения на просмотр источников данных.
	/// </summary>
	public static readonly PermissionName WatchItemSourcesDescription = PermissionName.Create(
		"Просмотр источников данных."
	);

	/// <summary>
	/// Разрешение на добавление данных в избранное.
	/// </summary>
	public static readonly PermissionName AddItemsToFavoritesName = PermissionName.Create("add.items.to.favorites");

	/// <summary>
	/// Описание разрешения на добавление данных в избранное.
	/// </summary>
	public static readonly PermissionName AddItemsToFavoritesDescription = PermissionName.Create(
		"Добавление данных в избранное."
	);

	/// <summary>
	/// Разрешение на доступ к телеметрии.
	/// </summary>
	public static readonly PermissionName AccessTelemetryName = PermissionName.Create("access.telemetry");

	/// <summary>
	/// Описание разрешения на доступ к телеметрии.
	/// </summary>
	public static readonly PermissionName AccessTelemetryDescription = PermissionName.Create(
		"Доступ к телеметрии (регистр активностей пользователей)."
	);
}
