using Identity.Domain.Contracts.Outbox;
using Identity.Domain.Contracts.Persistence;
using Identity.Infrastructure.Common;
using Identity.Infrastructure.Common.UnitOfWork;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Identity.WebApi.BackgroundServices;

public sealed class AccountsModuleOutboxProcessor(
    IEnumerable<IAccountOutboxMessagePublisher> publishers,
    NpgSqlConnectionFactory connectionFactory,
    Serilog.ILogger logger
    )
    : BackgroundService
{
    private IEnumerable<IAccountOutboxMessagePublisher> Publishers { get; } = publishers;
    private Serilog.ILogger Logger { get; } = logger.ForContext<AccountsModuleOutboxProcessor>();
    private NpgSqlConnectionFactory ConnectionFactory { get; } = connectionFactory;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Execute(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task Execute(CancellationToken ct)
    {
        await using NpgSqlSession session = new(ConnectionFactory);
        NpgSqlTransactionSource transactionSource = new(session, logger);
        IAccountsModuleUnitOfWork unitOfWork = CreateUnitOfWork(session);
        await using ITransactionScope transactionScope = await transactionSource.BeginTransaction(ct);

        try
        {
            IAccountModuleOutbox outbox = new AccountsModuleOutbox(session, unitOfWork);
            IdentityOutboxMessage[] messages = await GetMessages(outbox, ct);
            await PublishMessages(messages, ct);
            await unitOfWork.Save(messages, ct);
            Result commit = await transactionScope.Commit(ct);
        
            if (commit.IsFailure)
            {
                Logger.Fatal(commit.Error, "Error committing transaction.");
            }
            else
            {
                Logger.Information("Committed transaction for {Count} messages.", messages.Length);
            }
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Error processing outbox messages.");
        }
    }

    private IAccountsModuleUnitOfWork CreateUnitOfWork(NpgSqlSession session)
    {
        return new AccountsModuleUnitOfWork(
            new AccountsChangeTracker(session),
            new AccountTicketsChangeTracker(session),
            new PermissionsChangeTracker(session),
            new IdentityOutboxMessageChangeTracker(session)
        );
    }
    
    private async Task<IdentityOutboxMessage[]> GetMessages(IAccountModuleOutbox outbox, CancellationToken ct = default) =>
        await outbox.GetMany(
            new OutboxMessageSpecification()
            .OfLimit(50)
            .OfNotSentOnly()
            .OfWithLock(), ct);

    private async Task PublishMessages(IdentityOutboxMessage[] messages, CancellationToken ct = default)
    {
        foreach (IAccountOutboxMessagePublisher publisher in Publishers)
        {
            foreach (IdentityOutboxMessage message in messages)
            {
                try
                {
                    await publisher.Publish(message, ct);
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error publishing message {Message}", message);
                    message.IncreaseRetryCount();
                }
            }
        }
    }
}