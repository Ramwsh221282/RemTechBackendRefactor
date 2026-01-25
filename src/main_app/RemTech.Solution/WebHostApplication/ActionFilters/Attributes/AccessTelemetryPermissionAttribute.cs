using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class AccessTelemetryPermissionAttribute : TypeFilterAttribute
{
    public AccessTelemetryPermissionAttribute()
        : base(typeof(ShouldHaveAccessTelemetryPermissionFilter)) { }
}
