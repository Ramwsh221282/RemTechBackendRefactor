using Cleaners.Domain.Cleaners.Aggregate;
using Cleaners.Domain.Cleaners.Ports.Outbox;
using Cleaners.Domain.Cleaners.UseCases.CreateCleaner;
using Cleaners.Domain.Cleaners.UseCases.StartWork;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.DependencyInjection;
using Shared.Infrastructure.Module.Postgres;

namespace Cleaners.Tests.CleanerOutboxTests;

public sealed class CleanerOutboxTests : IClassFixture<CleanersTestHostFactory>
{
    private readonly IServiceProvider _sp;

    public CleanerOutboxTests(CleanersTestHostFactory factory) => _sp = factory.Services;

    [Fact]
    private async Task Ensure_Outbox_Has_Processed_Cleaner_Started_Message()
    {
        var cleaner = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<CreateCleanerCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new CreateCleanerCommand())
        );
        Assert.True(cleaner.IsSuccess);

        var started = await _sp.ScopedExecution(
            scope =>
                scope.GetService<
                    ICommandHandler<StartWorkCommand, Status<Domain.Cleaners.Aggregate.Cleaner>>
                >(),
            handler => handler.Handle(new StartWorkCommand(cleaner.Value.Id))
        );
        Assert.True(started.IsSuccess);

        bool persists = await EnsureOutboxMessagePersists();
        Assert.True(persists);
    }

    private async Task<bool> EnsureOutboxMessagePersists(int retryCount = 10)
    {
        int currentCounter = 0;
        while (currentCounter < retryCount)
        {
            await using var scope = _sp.CreateAsyncScope();
            var database = scope.GetService<PostgresDatabase>();
            const string sql = """
                SELECT * FROM cleaners_module.outbox
                WHERE processed IS NOT NULL
                """;
            using var connection = await database.ProvideConnection();
            var entries = await connection.QueryAsync<CleanerOutboxMessage>(sql);
            if (entries.Any())
                return true;
            await Task.Delay(TimeSpan.FromSeconds(5));
            currentCounter++;
        }

        return false;
    }
}
