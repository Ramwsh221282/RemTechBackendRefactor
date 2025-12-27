using Microsoft.Extensions.DependencyInjection;

namespace ParsingSDK.RabbitMq;

public static class FinishParserMessageInjection
{
    extension(IServiceCollection services)
    {
        public void AddFinishParserProducer()
        {
            services.AddSingleton<FinishParserProducer>();
        }
    }
}