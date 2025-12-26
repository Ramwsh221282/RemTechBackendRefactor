using Microsoft.Extensions.DependencyInjection;
using ParsersControl.Core;
using ParsersControl.Core.Contracts;
using ParsersControl.Infrastructure.Listeners;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.Parsers.Repository;
using ParsersControl.RabbitMQ.Listeners;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace ParsersControl.CompositionRoot;

public static class ParsersControlInjection
{
    extension(IServiceCollection services)
    {
        public void AddParsersControlModule()
        {
            services.RegisterPersistence();
            services.RegisterUseCaseHandler();
            services.AddParsersControlBackgroundServices();
            services.RegisterEventListeners();
        }

        private void RegisterEventListeners()
        {
            services.AddScoped<IOnParserSubscribedListener, OnParserSubscribedEventListener>();
            services.AddScoped<IOnParserStartedListener, OnParserStartedEventListener>();
        }
        
        private void RegisterPersistence()
        {
            services.AddScoped<ISubscribedParsersRepository, SubscribedParsersRepository>();
        }

        private void RegisterUseCaseHandler()
        {
            services.RegisterParserControlHandlers();
        }
    }
}