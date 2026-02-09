using ParserSubscriber.SubscribtionContext;
using ParserSubscriber.SubscribtionContext.Options;

namespace DromVehiclesParser.DependencyInjection;

public static class ParserSubscriptionInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterParserSubscription()
        {
            services.RegisterRabbitMqResponseReplyListeningOptions();
            services.RegisterParserSubscriber<RabbitMqRequestReplySubscriber>("drom_vehicles_parser");
        }

        private void RegisterRabbitMqResponseReplyListeningOptions()
        {
            services.AddOptions<RabbitMqRequestReplyResponseListeningQueueOptions>()
                .BindConfiguration(nameof(RabbitMqRequestReplyResponseListeningQueueOptions));
        }
    }
}