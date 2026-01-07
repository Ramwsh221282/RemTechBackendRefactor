using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class AddItemsToFavoritesPermissionAttribute : TypeFilterAttribute
{
    public AddItemsToFavoritesPermissionAttribute() : base(typeof(ShouldHaveAddItemsToFavoritesPermissionFilter))
    {
    }
}