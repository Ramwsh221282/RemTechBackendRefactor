using Identity.Domain.Accounts.Features.GivePermissions;
using Identity.Domain.Accounts.Features.RegisterAccount;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts;
using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Domain.Permissions;
using Identity.Domain.Permissions.Features.AddPermissions;
using Identity.Domain.Tickets;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Identity.Tests;

public static class IdentityModuleTestExtensions
{
    extension(IServiceProvider services)
    {
        public async Task<Result<Unit>> InvokeAccountRegistration(RegisterAccountCommand command)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            return await scope.ServiceProvider.GetRequiredService<ICommandHandler<RegisterAccountCommand, Unit>>()
                .Execute(command);
        }

        public async Task<Result<Permission>> AddPermission(string name, string description)
        {
            AddPermissionCommandPayload payload = new(name, description);
            IEnumerable<AddPermissionCommandPayload> payloads = [payload];
            Result<IEnumerable<Permission>> result = await services.AddPermissions(payloads);
            return result.IsFailure ? result.Error : result.Value.First();
        }

        public async Task<Result<IEnumerable<Permission>>> AddPermissions(
            IEnumerable<AddPermissionCommandPayload> payloads)
        {
            AddPermissionsCommand command = new(payloads);
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            return await scope.ServiceProvider.GetRequiredService<ICommandHandler<AddPermissionsCommand, IEnumerable<Permission>>>()
                .Execute(command);
        }
        
        public async Task<Result<Account>> GetAccountByEmail(string email)
        {
            AccountSpecification specification = new AccountSpecification().WithEmail(email);
            return await services.GetAccount(specification);
        }

        public async Task<Result<Account>> GetAccountByName(string name)
        {
            AccountSpecification specification = new AccountSpecification().WithLogin(name);
            return await services.GetAccount(specification);
        }

        public async Task<Result<Account>> GivePermissions(Guid accountId, IEnumerable<Guid> permissionIds)
        {
            GivePermissionsCommand command = new(accountId, permissionIds.Select(id => new GivePermissionsPermissionsPayload(id)));
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            return await scope.ServiceProvider.GetRequiredService<ICommandHandler<GivePermissionsCommand, Account>>()
                .Execute(command);
        }

        public async Task<IEnumerable<Permission>> GetPermissions(IEnumerable<PermissionSpecification>? specs = null)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            IPermissionsRepository permissions = scope.ServiceProvider.GetRequiredService<IPermissionsRepository>();
            return specs is null ? await permissions.GetMany([], CancellationToken.None) : await permissions.GetMany(specs, CancellationToken.None);
        }
        
        public async Task<Result<Account>> GivePermission(Guid accountId, Guid permissionId)
        {
            return await services.GivePermissions(accountId, [permissionId]);
        }
        
        private async Task<Result<Account>> GetAccount(AccountSpecification specification)
        {
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            IAccountsRepository accounts = scope.ServiceProvider.GetRequiredService<IAccountsRepository>();
            return await accounts.Get(specification, CancellationToken.None);
        }

        public async Task<OutboxMessage[]> GetOutboxMessagesOfType(string type)
        {
            OutboxMessageSpecification spec = new OutboxMessageSpecification().OfType(type);
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            IAccountModuleOutbox outbox = scope.ServiceProvider.GetRequiredService<IAccountModuleOutbox>();
            return await outbox.GetMany(spec, CancellationToken.None);
        }

        public async Task<Result<AccountTicket>> GetTicketOfPurpose(string purpose)
        {
            AccountTicketSpecification spec = new AccountTicketSpecification().WithPurpose(purpose);
            await using AsyncServiceScope scope = services.CreateAsyncScope();
            IAccountTicketsRepository tickets = scope.ServiceProvider.GetRequiredService<IAccountTicketsRepository>();
            return await tickets.Get(spec, CancellationToken.None);
        }
    }
}