using WebHostApplication.ActionFilters.Filters;

namespace WebHostApplication.Injection;

public static class ActionFiltersInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterActionFilters()
        {
            services.RegisterVerifyTokenFilter();
        }

        private void RegisterVerifyTokenFilter()
        {
            services.AddScoped<VerifyTokenFilter>();
            services.AddScoped<ShouldHaveAccessTelemetryPermissionFilter>();
            services.AddScoped<ShouldHaveAddItemsToFavoritesPermissionFilter>();
            services.AddScoped<ShouldHaveIdentityManagementPermissionFilter>();
            services.AddScoped<ShouldHaveNotificationsManagementPermissionFilter>();
            services.AddScoped<ShouldHaveParserManagementPermissionFilter>();
            services.AddScoped<ShouldHaveWatchItemSourcesPermissionFilter>();
        }
    }
}