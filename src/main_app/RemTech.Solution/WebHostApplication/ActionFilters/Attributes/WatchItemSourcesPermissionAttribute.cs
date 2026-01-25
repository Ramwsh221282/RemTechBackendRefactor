using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class WatchItemSourcesPermissionAttribute : TypeFilterAttribute
{
    public WatchItemSourcesPermissionAttribute()
        : base(typeof(ShouldHaveWatchItemSourcesPermissionFilter)) { }
}
