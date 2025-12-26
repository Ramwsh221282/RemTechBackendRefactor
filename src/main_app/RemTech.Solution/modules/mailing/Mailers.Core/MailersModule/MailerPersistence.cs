using Mailers.Core.EmailsModule;

namespace Mailers.Core.MailersModule;

public delegate Task<Result<Unit>> InsertMailer(Mailer mailer, CancellationToken ct);
public delegate Task<Result<Unit>> DeleteMailer(Mailer mailer, CancellationToken ct);
public delegate Task<bool> HasUniqueMailerEmail(Email email, CancellationToken ct);
public delegate Task<Result<Unit>> UpdateMailer(Mailer mailer, CancellationToken ct);
public delegate Task<Optional<Mailer>> GetMailer(QueryMailerArguments args, CancellationToken ct);
public delegate Task<IEnumerable<Mailer>> GetManyMailers(QueryMailerArguments args, CancellationToken ct);
public delegate Task<Result<Unit>> InsertMailerSending(MailerSending sending, CancellationToken ct);
public sealed record QueryMailerArguments(
    Guid? Id = null, 
    string? Email = null, 
    string? SmtpPassword = null, 
    int? SendLimit = null, 
    int? SendCurrent = null,
    bool WithLock = false);