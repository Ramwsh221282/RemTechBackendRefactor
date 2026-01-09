using System.Reflection;
using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using ParsersControl.Infrastructure.Listeners;
using ParsersControl.Infrastructure.Migrations;
using ParsersControl.Infrastructure.Parsers.CacheInvalidators;
using ParsersControl.Infrastructure.Parsers.Commands.SubscribeParser;
using ParsersControl.Infrastructure.Parsers.Queries.GetParser;
using ParsersControl.Infrastructure.Parsers.Queries.GetParsers;
using ParsersControl.Infrastructure.Parsers.Repository;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace ParsersControl.WebApi.Extensions;

public static class ParsersModuleInjection
{
    extension(IServiceCollection services)
    {
        public void InjectParsersControlModule()
        {
            services.AddDomainLayer();
            services.AddInfrastructureLayer();
        }
        
        public void AddParsersControlModule(bool isDevelopment)
        {
            services.AddSharedInfrastructure(isDevelopment);
            services.AddInfrastructureLayer();
            services.AddDomainLayer();
        }
  
        private void AddSharedInfrastructure(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddMigrations([typeof(ParsersTableMigration).Assembly]);
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
            }
            
            services.AddPostgres();
            services.AddRabbitMq();
        }

        private void AddDomainLayer()
        {
            Assembly assembly = typeof(SubscribedParser).Assembly;
            new HandlersRegistrator(services).FromAssemblies([assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .RequireRegistrationOf(typeof(IEventTransporter<,>))
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }
        
        private void AddInfrastructureLayer()
        {
            services.AddEventListeners();
            services.AddRepositories();
            services.AddCacheInvalidators();
            services.AddQueryHandlers();
            services.UseCachedQueryHandlers();
        }

        private void AddCacheInvalidators()
        {
            Assembly assembly = typeof(SubscribeParserCacheInvalidator).Assembly;
            services.Scan(s => s.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(ICacheInvalidator<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
            services.AddScoped<CachedParserArrayInvalidator>();
            services.AddScoped<ParserCacheRecordInvalidator>();
        }

        private void AddQueryHandlers()
        {
            Assembly assembly = typeof(GetParsersQuery).Assembly;
            services.Scan(s => s.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }

        private void UseCachedQueryHandlers()
        {
            services.Decorate(typeof(IQueryHandler<GetParsersQuery, IEnumerable<ParserResponse>>), typeof(GetParsersCachedQueryHandler));
            services.Decorate(typeof(IQueryHandler<GetParserQuery, ParserResponse?>), typeof(GetParserCachedQueryHandler));
        }
        
        private void AddEventListeners()
        {
            services.AddScoped<IOnParserSubscribedListener, OnParserSubscribedEventListener>();
            services.AddScoped<IOnParserStartedListener, OnParserStartedEventListener>();
            services.Scan(x => x.FromAssemblies(typeof(ParserSubscribeConsumer).Assembly)
                .AddClasses(classes => classes.AssignableTo<IConsumer>())
                .AsSelfWithInterfaces()
                .WithTransientLifetime());
        }

        private void AddRepositories()
        {
            services.AddScoped<ISubscribedParsersRepository, SubscribedParsersRepository>();
            services.AddScoped<ISubscribedParsersCollectionRepository, SubscribedParsersCollectionRepository>();
        }
    }
}