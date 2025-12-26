using Microsoft.Extensions.DependencyInjection;

namespace ParsersControl.RabbitMQ.Listeners;

public static class ParsersControlBackgroundServicesInjection
{
    extension(IServiceCollection services)
    {
        public void AddParsersControlBackgroundServices()
        {
            services.AddHostedService<ParserSubscribeListener>();
        }
    }
}