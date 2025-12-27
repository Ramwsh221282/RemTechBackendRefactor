using ParserSubscriber.SubscribtionContext;
using ParserSubscriber.SubscribtionContext.Options;

namespace RemTechAvitoVehiclesParser;

public static class ParserSubscriptionInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterParserSubscription()
        {
            services.AddOptions<RabbitMqRequestReplyResponseListeningQueueOptions>().BindConfiguration(nameof(RabbitMqRequestReplyResponseListeningQueueOptions));
            services.RegisterParserSubscriber<RabbitMqRequestReplySubscriber>("avito_parser_module");
        }
    }
}