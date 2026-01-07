using Microsoft.AspNetCore.Mvc;
using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.ActionFilters.Attributes;

public sealed class VerifyTokenAttribute : TypeFilterAttribute
{
    public VerifyTokenAttribute() : base(typeof(VerifyTokenFilter))
    {
    }
}