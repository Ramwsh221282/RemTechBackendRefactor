using Mailing.Module.MailersContext.MetadataContext;
using Mailing.Module.MailersContext.StatisticsContext;
using Npgsql;
using RemTech.Core.Shared.Result;

namespace Mailing.Module.MailersContext;

internal sealed class Mailer(MailerMetadata metadata, MailerStatistics statistics)
{
    public static Status<Mailer> Create(string email, string password)
    {
    }
}

internal sealed class NpgSqlMailer(
    Lazy<NpgsqlConnection> connectionCache,
    MailerMetadata metadata,
    MailerStatistics statistics)
{
}