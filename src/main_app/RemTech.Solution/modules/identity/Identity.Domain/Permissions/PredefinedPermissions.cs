namespace Identity.Domain.Permissions;

public static class PredefinedPermissions
{
	public static readonly PermissionName NotificationsManagementName = PermissionName.Create(
		"notifications.management"
	);
	public static readonly PermissionName NotificationsManagementDescription = PermissionName.Create(
		"Управление рассылкой уведомлений."
	);
	public static readonly PermissionName IdentityManagementName = PermissionName.Create("identity.management");
	public static readonly PermissionName IdentityManagementDescription = PermissionName.Create(
		"Управление учетными записями."
	);
	public static readonly PermissionName ParserManagementName = PermissionName.Create("parser.management");
	public static readonly PermissionName ParserManagementDescription = PermissionName.Create("Управление парсерами.");
	public static readonly PermissionName WatchItemSourcesName = PermissionName.Create("watch.item.sources");
	public static readonly PermissionName WatchItemSourcesDescription = PermissionName.Create(
		"Просмотр источников данных."
	);
	public static readonly PermissionName AddItemsToFavoritesName = PermissionName.Create("add.items.to.favorites");
	public static readonly PermissionName AddItemsToFavoritesDescription = PermissionName.Create(
		"Добавление данных в избранное."
	);
	public static readonly PermissionName AccessTelemetryName = PermissionName.Create("access.telemetry");
	public static readonly PermissionName AccessTelemetryDescription = PermissionName.Create(
		"Доступ к телеметрии (регистр активностей пользователей)."
	);
}
