using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.Database;
using Serilog;

namespace ParserSubscriber.SubscribtionContext;

public static class ParserSubscriberInjection
{
    extension(IServiceCollection services)
    {
        public void RegisterParserSubscriber<T>(string schemaName)
            where T : class, IParserSubscriber
        {
            services.AddTransient<IParserSubscriber, T>();
            services.RegisterSubscriptionStorage(schemaName);
            services.RegisterPublisher();
        }
        
        private void RegisterSubscriptionStorage(string schemaName)
        {
            services.AddSingleton<SubscriptionStorage>(sp =>
            {
                ILogger? logger = sp.GetService<ILogger>();
                NpgSqlConnectionFactory factory = sp.GetRequiredService<NpgSqlConnectionFactory>();
                SubscriptionStorage storage = new(factory, logger);
                storage.SetSchema(schemaName);
                return storage;
            });
        }

        private void RegisterPublisher() => services.AddTransient<ParserSubscriptionPublisher>();
    }

    extension(IServiceProvider services)
    {
        public async Task RunParserSubscription()
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            IParserSubscriber subscriber = scope.ServiceProvider.GetRequiredService<IParserSubscriber>();
            await subscriber.Subscribe();
        }
    }
}