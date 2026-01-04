using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.PasswordRequirements;
using Identity.Infrastructure.Accounts;
using Identity.Infrastructure.Common;
using Identity.Infrastructure.Common.Migrations;
using Identity.Infrastructure.Common.UnitOfWork;
using Identity.Infrastructure.Permissions;
using Identity.Infrastructure.Tickets;
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
                .AlsoAddValidators()
                .AlsoAddDecorators()
                .AlsoUseDecorators()
                .Invoke();
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
        }
    }
}