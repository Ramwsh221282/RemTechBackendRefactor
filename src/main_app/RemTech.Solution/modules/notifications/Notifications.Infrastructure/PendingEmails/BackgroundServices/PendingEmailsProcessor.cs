using Microsoft.Extensions.Hosting;
using Notifications.Core.Common.Contracts;
using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using Notifications.Core.PendingEmails;
using Notifications.Core.PendingEmails.Contracts;
using Notifications.Infrastructure.Common;
using Notifications.Infrastructure.EmailSending;
using Notifications.Infrastructure.Mailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;
using RemTech.SharedKernel.Infrastructure.Database;

namespace Notifications.Infrastructure.PendingEmails.BackgroundServices;

public sealed class PendingEmailsProcessor(
    NpgSqlConnectionFactory npgSql,
    Serilog.ILogger logger,
    IMailerCredentialsCryptography cryptography,
    EmailSender sender
) : BackgroundService
{
    private NpgSqlConnectionFactory NpgSql { get; } = npgSql;
    private Serilog.ILogger Logger { get; } = logger.ForContext<PendingEmailsProcessor>();
    private IMailerCredentialsCryptography Cryptography { get; } = cryptography;
    private EmailSender Sender { get; } = sender;

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
        Logger.Information("Pending emails processor started.");
        await using NpgSqlSession session = new(NpgSql);
        INotificationsModuleUnitOfWork unitOfWork = CreateUnitOfWork(session);
        IPendingEmailNotificationsRepository pendingEmailsRepostiory =
            new PendingEmailNotificationsRepository(session, unitOfWork);
        IMailersRepository mailersRepository = new MailersRepository(session, unitOfWork);
        NpgSqlTransactionSource transactionSource = new(session);
        await using ITransactionScope scope = await transactionSource.BeginTransaction(ct);

        try
        {
            Mailer[] mailers = await GetMailers(mailersRepository, ct);
            if (mailers.Length == 0)
            {
                Logger.Warning("No mailers found.");
                return;
            }

            PendingEmailNotification[] pendingEmails = await GetPendingEmails(
                pendingEmailsRepostiory,
                ct
            );
            if (pendingEmails.Length == 0)
            {
                Logger.Warning("No pending emails found.");
                return;
            }

            await DecryptMailers(mailers, ct);
            await PublishPendingEmails(GetRandomMailer(mailers), pendingEmails, ct);

            IEnumerable<PendingEmailNotification> sent = pendingEmails.Where(e => e.WasSent);
            int removed = await pendingEmailsRepostiory.Remove(sent, ct);
            Result commit = await scope.Commit(ct);
            if (commit.IsFailure)
            {
                Logger.Fatal(commit.Error, "Error committing transaction.");
            }
            else
            {
                Logger.Information("Committed transaction for {Count} messages.", removed);
            }
        }
        catch (Exception e)
        {
            Logger.Fatal(e, "Error processing pending emails.");
        }
    }

    private Mailer GetRandomMailer(Mailer[] mailers)
    {
        int index = new Random().Next(mailers.Length);
        return mailers[index];
    }

    private async Task PublishPendingEmails(
        Mailer mailer,
        PendingEmailNotification[] pendingEmails,
        CancellationToken ct
    )
    {
        for (int i = 0; i < pendingEmails.Length; i++)
        {
            bool processed = await Sender.Process(mailer, pendingEmails[i], ct);
            if (processed)
                pendingEmails[i].MarkSent();
        }
    }

    private async Task DecryptMailers(Mailer[] mailers, CancellationToken ct)
    {
        for (int i = 0; i < mailers.Length; i++)
            await mailers[i].DecryptCredentials(Cryptography, ct);
    }

    private async Task<Mailer[]> GetMailers(IMailersRepository repository, CancellationToken ct)
    {
        MailersSpecification specification = new MailersSpecification().WithLockRequired();
        return await repository.GetMany(specification, ct);
    }

    private async Task<PendingEmailNotification[]> GetPendingEmails(
        IPendingEmailNotificationsRepository repository,
        CancellationToken ct
    )
    {
        PendingEmailNotificationsSpecification specification =
            new PendingEmailNotificationsSpecification().OfNotSentOnly().WithLock().WithLimit(50);
        return await repository.GetMany(specification, ct);
    }

    private INotificationsModuleUnitOfWork CreateUnitOfWork(NpgSqlSession session) =>
        new NotificationsModuleUnitOfWork(
            new MailersChangeTracker(session),
            new PendingEmailsChangeTracker(session)
        );
}
