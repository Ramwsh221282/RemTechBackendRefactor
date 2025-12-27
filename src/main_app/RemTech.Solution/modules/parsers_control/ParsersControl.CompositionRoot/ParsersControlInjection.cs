using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core;
using ParsersControl.Core.Contracts;
using ParsersControl.Infrastructure.Listeners;
using ParsersControl.Infrastructure.Parsers.Repository;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.CompositionRoot;

public static class ParsersControlInjection
{
    extension(IServiceCollection services)
    {
        public void AddParsersControlModule()
        {
            services.RegisterPersistence();
            services.RegisterUseCaseHandler();
            services.RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            services.AddScoped<IOnParserSubscribedListener, OnParserSubscribedEventListener>();
            services.AddScoped<IOnParserStartedListener, OnParserStartedEventListener>();
            services.Scan(x => x.FromAssemblies(typeof(ParserSubscribeConsumer).Assembly)
                .AddClasses(classes => classes.AssignableTo<IConsumer>())
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }
        
        private void RegisterPersistence() => services.AddScoped<ISubscribedParsersRepository, SubscribedParsersRepository>();

        private void RegisterUseCaseHandler() => services.RegisterParserControlHandlers();
    }
}