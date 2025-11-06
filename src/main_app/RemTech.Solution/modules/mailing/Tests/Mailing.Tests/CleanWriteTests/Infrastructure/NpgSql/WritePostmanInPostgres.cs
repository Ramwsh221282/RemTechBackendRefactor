using Mailing.Domain.General;
using Mailing.Tests.CleanWriteTests.Models;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.NpgSql;

public sealed class WritePostmanInPostgres : IWritePostmanInfrastructureCommand, IDisposable
{
    private readonly NpgSqlPostman _postman;
    private event EventHandler<EventArgs> Event;

    public WritePostmanInPostgres(PostgresDatabase db, CancellationToken ct, DelayedUnitStatus container)
    {
        _postman = new NpgSqlPostman(db, ct);
        Event += async (_, _) =>
        {
            try
            {
                await InsertPostman();
                container.Accept(Unit.Value);
            }
            catch (ConflictOperationException ex)
            {
                container.Accept(Error.Conflict(ex.Message));
            }
            catch (Exception)
            {
                container.Accept(Error.Internal("Не удается сохранить Postman в БД. Ошибка на стороне приложения."));
            }
        };
    }

    public void Execute(in TestPostmanMetadata metadata, in TestPostmanStatistics statistics)
    {
        metadata.Write(_postman);
        statistics.Write(_postman);
        Event?.Invoke(this, EventArgs.Empty);
    }

    private async Task InsertPostman()
    {
        if (!await _postman.HasUniqueEmail())
            throw new ConflictOperationException("Postman с такой почтой уже существует.");
        await _postman.Save();
    }

    public void Dispose() => _postman.Dispose();
}