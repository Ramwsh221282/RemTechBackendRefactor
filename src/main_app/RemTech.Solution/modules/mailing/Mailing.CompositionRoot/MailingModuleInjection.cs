using Microsoft.Extensions.DependencyInjection;

namespace Mailing.CompositionRoot;

public static class MailingModuleInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailingModule()
        {
            services.AddMailingApplicationDependencies();
            services.AddMailingInfrastructureDependencies();
            services.AddMailerPresentationDependencies();
        }
    }
}