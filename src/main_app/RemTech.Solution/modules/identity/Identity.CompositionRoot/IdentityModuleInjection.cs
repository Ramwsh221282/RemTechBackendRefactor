using Identity.Contracts.Accounts;
using Identity.Gateways.Accounts.Activate;
using Identity.Gateways.Accounts.AddAccount;
using Identity.Gateways.Accounts.ChangeEmail;
using Identity.Gateways.Accounts.ChangePassword;
using Identity.Gateways.Accounts.RequireActivation;
using Identity.Gateways.Accounts.RequirePasswordReset;
using Identity.Gateways.Accounts.Responses;
using Identity.Gateways.Common;
using Identity.Infrastructure;
using Identity.Infrastructure.Accounts;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Infrastructure.NpgSql;

namespace Identity.CompositionRoot;

public static class IdentityModuleInjection
{
    extension(IServiceCollection services)
    {
        public void AddIdentityModule()
        {
            services.RegisterAccountContextDependencies();
        }
        
        private void RegisterAccountContextDependencies()
        {
            services.AddSingleton<IAccountEncrypter, AesAccountCryptography>();
            services.AddSingleton<IAccountDecrypter, AesAccountCryptography>();
            services.AddScoped<IAccountPersister, NpgSqlAccountPersister>();
            services.AddTransient<IDbUpgrader, IdentityModuleDbUpgrader>();
            
            services.AddScoped<IGateway<ActivateAccountRequest, AccountResponse>, ActivateAccountGateway>();
            services.AddScoped<IGateway<AddAccountRequest, AccountResponse>, AddAccountGateway>();
            services.AddScoped<IGateway<ChangeAccountEmailRequest, AccountResponse>,  ChangeAccountEmailGateway>();
            services.AddScoped<IGateway<ChangeAccountPasswordRequest, AccountResponse>,  ChangeAccountPasswordGateway>();
            services.AddScoped<IGateway<RequireActivationRequest, RequireActivationResponse>, RequireActivationGateway>();
            services.AddScoped<IGateway<RequirePasswordResetRequest, RequirePasswordResetResponse>, RequirePasswordResetGateway>();

            services.Decorate<
                IGateway<ActivateAccountRequest, AccountResponse>, 
                TransactionalGateway<ActivateAccountRequest, AccountResponse>>();
            
            services.Decorate<IGateway<ChangeAccountEmailRequest, AccountResponse>,
                TransactionalGateway<ChangeAccountEmailRequest, AccountResponse>>();
            
            services.Decorate<
                IGateway<ChangeAccountPasswordRequest, AccountResponse>,
                TransactionalGateway<ChangeAccountPasswordRequest, AccountResponse>>();
        }
    }
}