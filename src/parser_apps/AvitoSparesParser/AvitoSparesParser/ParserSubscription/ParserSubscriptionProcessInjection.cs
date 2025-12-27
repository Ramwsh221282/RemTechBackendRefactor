using ParserSubscriber.SubscribtionContext;
using ParserSubscriber.SubscribtionContext.Options;

namespace AvitoSparesParser.ParserSubscription;

public static class ParserSubscriptionProcessInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterParserSubscriptionProcess()
        {
            services.AddOptions<RabbitMqRequestReplyResponseListeningQueueOptions>()
                .BindConfiguration(nameof(RabbitMqRequestReplyResponseListeningQueueOptions));
            services.RegisterParserSubscriber<RabbitMqRequestReplySubscriber>(schemaName: "avito_spares_parser");
        }
    }
}
