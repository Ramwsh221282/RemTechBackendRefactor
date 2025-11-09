using Mailers.Core.MailersContext;

namespace Mailers.Persistence.NpgSql;

public sealed class TableMailer
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string SmtpPassword { get; init; }
    public required int SendLimit { get; init; }
    public required int SendAtThisMoment { get; init; }

    public Mailer ToMailer()
    {
        var meta = new MailerMetadata(Id, new Email(Email), new SmtpPassword(SmtpPassword));
        var stats = new MailerStatistics(SendLimit, SendAtThisMoment);
        return new Mailer(meta, stats);
    }
}