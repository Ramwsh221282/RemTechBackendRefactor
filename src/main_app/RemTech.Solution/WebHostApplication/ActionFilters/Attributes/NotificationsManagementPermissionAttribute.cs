using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class NotificationsManagementPermissionAttribute : TypeFilterAttribute
{
	public NotificationsManagementPermissionAttribute()
		: base(typeof(ShouldHaveNotificationsManagementPermissionFilter)) { }
}
