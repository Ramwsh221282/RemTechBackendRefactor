using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class IdentityManagementPermissionAttribute : TypeFilterAttribute
{
    public IdentityManagementPermissionAttribute() : base(typeof(ShouldHaveIdentityManagementPermissionFilter))
    {
    }
}