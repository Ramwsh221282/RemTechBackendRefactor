using Microsoft.Extensions.DependencyInjection;

namespace ParsingSDK.RabbitMq;

public static class AddContainedItemsInjection
{
    extension(IServiceCollection services)
    {
        public void AddContainedItemsProducer()
        {
            services.AddSingleton<AddContainedItemProducer>();
        }
    }
}