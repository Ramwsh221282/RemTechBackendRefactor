using Mailing.Module.Domain.Models;
using Mailing.Module.Infrastructure.NpgSql.Adapters.SearchCriteria;

namespace Mailing.Module.Infrastructure.NpgSql.Adapters.Storage.Decorators;

internal sealed class LoggingMailersStorage(Serilog.ILogger logger, IMailersStorage<PgMailerSearchCriteria> origin)
    : MailersStorageEnvelope(origin)
{
    public override async Task Add(IMailer postman, CancellationToken ct = default)
    {
        logger.Information("Adding postman begin.");
        await base.Add(postman, ct);
        logger.Information("Adding postman completed.");
    }

    public override async Task Remove(IMailer postman, CancellationToken ct = default)
    {
        logger.Information("Removing postman begin.");
        await base.Remove(postman, ct);
        logger.Information("Removing postman completed.");
    }

    public override async Task<IMailer> Find(PgMailerSearchCriteria criteria, CancellationToken ct = default)
    {
        logger.Information("Finding postman begin.");
        IMailer postman = await base.Find(criteria, ct);
        if (postman is EmptyMailer)
            logger.Warning("Postman is {Type}", nameof(EmptyMailer));
        logger.Information("Finding postman completed.");
        return postman;
    }
}