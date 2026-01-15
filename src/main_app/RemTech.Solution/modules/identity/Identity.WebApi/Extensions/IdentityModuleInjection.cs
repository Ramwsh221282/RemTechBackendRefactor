using System.Reflection;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Jwt;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Infrastructure.Accounts;
using Identity.Infrastructure.Accounts.Commands.ConfirmTicket;
using Identity.Infrastructure.Accounts.Queries.GetUser;
using Identity.Infrastructure.Common;
using Identity.Infrastructure.Common.BackgroundServices;
using Identity.Infrastructure.Common.Migrations;
using Identity.Infrastructure.Common.UnitOfWork;
using Identity.Infrastructure.Permissions;
using Identity.Infrastructure.RabbitMq.Producers;
using Identity.Infrastructure.Tickets;
using Identity.Infrastructure.Tokens;
using Identity.Infrastructure.Tokens.BackgroundServices;
using Identity.WebApi.BackgroundServices;
using Identity.WebApi.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;
using RemTech.SharedKernel.Infrastructure.Redis;

namespace Identity.WebApi.Extensions;

public static class IdentityModuleInjection
{
    extension(IServiceCollection services)
    {
        public void InjectIdentityModule()
        {
            services.AddDomain();
            services.AddInfrastructure();
        }

        public void RegisterIdentityModule(bool isDevelopment)
        {
            services.AddSharedDependencies(isDevelopment);
            services.AddDomain();
            services.AddInfrastructure();
        }

        private void AddSharedDependencies(bool isDevelopment)
        {
            services.RegisterLogging();
            if (isDevelopment)
            {
                services.AddMigrations([typeof(IdentityModuleSchemaMigration).Assembly]);
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                services.AddAesEncryptionOptionsFromAppsettings();
                SuperUserCredentialsOptions.AddFromAppsettings(services);
                services.AddBcryptWorkFactorOptionsFromAppsettings();
                services.AddJwtOptionsFromAppsettings();
                services.AddCachingOptionsFromAppsettings();
            }

            services.RegisterHybridCache();
            services.AddPostgres();
            services.AddRabbitMq();
            RemTech.SharedKernel.Infrastructure.AesEncryption.AesCryptographyExtensions.AddAesCryptography(
                services
            );
        }

        private void AddDomain()
        {
            services.AddDomainHandlers();
            services.AddPasswordRequirements();
        }

        private void AddPasswordRequirements()
        {
            services.AddSingleton<IAccountPasswordRequirement, DigitPasswordRequirement>();
            services.AddSingleton<IAccountPasswordRequirement, LowercasePasswordRequirement>();
            services.AddSingleton<IAccountPasswordRequirement, MinLengthPasswordRequirement>();
            services.AddSingleton<
                IAccountPasswordRequirement,
                SpecialCharacterPasswordRequirement
            >();
            services.AddSingleton<IAccountPasswordRequirement, UppercasePasswordRequirement>();
        }

        private void AddDomainHandlers()
        {
            new HandlersRegistrator(services)
                .FromAssemblies([typeof(Account).Assembly])
                .RequireRegistrationOf(typeof(ICommandHandler<,>))
                .AlsoAddDomainEventHandlers()
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
        }

        private void AddBackgroundServices()
        {
            services.AddHostedService<SuperUserAccountRegistrationOnStartupBackgroundService>();
            services.AddHostedService<SuperUserAccountPermissionsUpdateBackgroundServices>();
            services.AddHostedService<AccountsModuleOutboxProcessor>();
            services.AddHostedService<ExpiredTokensCleanerService>();
            services.AddHostedService<AccountsModuleOutboxCleaner>();
        }

        public void AddInfrastructure()
        {
            services.AddPasswordHasher();
            services.AddRepositories();
            services.AddChangeTracker();
            services.AddOutboxMessagePublishers();
            services.AddBackgroundServices();
            services.AddJwt();
            services.UseCacheInvalidatingHandlers();
            services.UseCacheOnRepositories();
            services.UseCacheOnQueryHandlers();
        }

        private void AddPasswordHasher()
        {
            services.AddSingleton<IPasswordHasher, PasswordHasher>();
        }

        private void UseCacheOnRepositories()
        {
            services.Decorate<IAccessTokensRepository, CachedAccessTokenRepository>();
        }

        private void AddChangeTracker()
        {
            services.AddScoped<AccountsChangeTracker>();
            services.AddScoped<AccountTicketsChangeTracker>();
            services.AddScoped<PermissionsChangeTracker>();
            services.AddScoped<IdentityOutboxMessageChangeTracker>();
            services.AddScoped<IAccountsModuleUnitOfWork, AccountsModuleUnitOfWork>();
        }

        private void AddRepositories()
        {
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IAccountTicketsRepository, AccountTicketsRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<IRefreshTokensRepository, RefreshTokensRepository>();
            services.AddScoped<IAccessTokensRepository, AccessTokensRepository>();
            services.AddScoped<IAccountModuleOutbox, AccountsModuleOutbox>();
        }

        private void UseCacheOnQueryHandlers()
        {
            services.TryDecorate<
                IQueryHandler<GetUserQuery, UserAccountResponse?>,
                GetUserCachedQueryHandler
            >();
        }

        private void AddJwt()
        {
            services.AddSingleton<IJwtTokenManager, JwtTokenManager>();
        }

        private void AddOutboxMessagePublishers()
        {
            Assembly assembly = typeof(NewAccountRegisteredProducer).Assembly;
            _ = services.Scan(s =>
                s.FromAssemblies(assembly)
                    .AddClasses(classes => classes.AssignableTo<IAccountOutboxMessagePublisher>())
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime()
            );
        }
    }
}
