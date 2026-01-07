using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class ParserManagementPermissionAttribute : TypeFilterAttribute
{
    public ParserManagementPermissionAttribute() : base(typeof(ShouldHaveParserManagementPermissionFilter))
    {
    }
}