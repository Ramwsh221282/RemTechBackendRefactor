using Identity.Application.EventListeners;
using Identity.Application.Features;
using Identity.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace Identity.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddIdentityModule()
        {
            services.AddIdentityPersistenceModule();
            services.AddScoped<RegisterIdentitySubject>();
            services.RegisterEventListeners();
            services.RegisterEventsRegistry();
        }
    }
}