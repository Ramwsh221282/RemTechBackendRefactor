using Notifications.Core.Mailers.Contracts;

namespace Notifications.Core.Mailers;

public sealed class Mailer(MailerId id, MailerCredentials credentials)
{
    private Mailer(Mailer mailer)
        : this(mailer.Id, mailer.Credentials) { }

    public MailerId Id { get; private set; } = id;
    public MailerCredentials Credentials { get; private set; } = credentials;

    public void ChangeCredentials(MailerCredentials credentials) => Credentials = credentials;

    public static Mailer CreateNew(MailerCredentials credentials)
    {
        MailerId id = MailerId.New();
        return new Mailer(id, credentials);
    }

    public async Task EncryptCredentials(IMailerCredentialsCryptography cryptography, CancellationToken ct = default) =>
        Credentials = await Credentials.Encrypt(cryptography, ct);

    public async Task DecryptCredentials(IMailerCredentialsCryptography cryptography, CancellationToken ct = default) =>
        Credentials = await Credentials.Decrypt(cryptography, ct);

    public Mailer Copy() => new(this);
}
