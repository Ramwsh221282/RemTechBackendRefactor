using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.PasswordRequirements;
using Identity.Infrastructure.Accounts;
using Identity.Infrastructure.Common;
using Identity.Infrastructure.Common.Migrations;
using Identity.Infrastructure.Common.UnitOfWork;
using Identity.Infrastructure.Permissions;
using Identity.Infrastructure.Tickets;
using Identity.WebApi.BackgroundServices;
using Identity.WebApi.Options;
using RemTech.SharedKernel.Configurations;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Logging;
using RemTech.SharedKernel.Infrastructure.Database;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.WebApi.Extensions;

public static class IdentityModuleInjection
{
    extension(IServiceCollection services)
    {
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
                services.AddNpgSqlOptionsFromAppsettings();
                services.AddRabbitMqOptionsFromAppsettings();
                services.AddAesEncryptionOptionsFromAppsettings();
                SuperUserCredentialsOptions.AddFromAppsettings(services);
            }
            
            services.AddPostgres();
            services.AddRabbitMq();
            RemTech.SharedKernel.Infrastructure.AesEncryption.AesCryptographyExtensions.AddAesCryptography(services);
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
            services.AddSingleton<IAccountPasswordRequirement, SpecialCharacterPasswordRequirement>();
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
        }
        
        private void AddInfrastructure()
        {
            services.AddScoped<IAccountsRepository, AccountsRepository>();
            services.AddScoped<IAccountTicketsRepository, AccountTicketsRepository>();
            services.AddScoped<IPermissionsRepository, PermissionsRepository>();
            services.AddScoped<AccountsChangeTracker>();
            services.AddScoped<AccountTicketsChangeTracker>();
            services.AddScoped<PermissionsChangeTracker>();
            services.AddScoped<IAccountsModuleUnitOfWork, AccountsModuleUnitOfWork>();
            services.AddScoped<IAccountModuleOutbox, AccountsModuleOutbox>();
            services.AddMigrations([typeof(IdentityModuleSchemaMigration).Assembly]);
            services.AddSingleton<IPasswordCryptography, PasswordCryptography>();
            services.AddBackgroundServices();
        }
    }
}