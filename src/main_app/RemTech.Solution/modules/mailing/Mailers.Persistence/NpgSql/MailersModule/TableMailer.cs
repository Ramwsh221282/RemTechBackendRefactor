using Mailers.Core.MailersModule;

namespace Mailers.Persistence.NpgSql.MailersModule;

public sealed record TableMailer
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string SmtpPassword { get; init; }
    public required int SendLimit { get; init; }
    public required int SendAtThisMoment { get; init; }

    public Mailer ToMailer()
    {
        return Mailer.Construct(Email, SmtpPassword, SendLimit, SendAtThisMoment, Some(Id));
    }
}