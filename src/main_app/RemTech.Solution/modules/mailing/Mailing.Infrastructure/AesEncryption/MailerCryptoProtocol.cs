using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace Mailing.Infrastructure.AesEncryption;

public sealed class MailerCryptoProtocol(AesCryptography cryptography) 
    : EncryptMailerSmtpPasswordProtocol, 
        DecryptMailerSmtpPasswordProtocol
{
    public async Task<Mailer> WithEncryptedPassword(Mailer mailer, CancellationToken ct)
    { 
        string encryptedText = await cryptography.EncryptText(mailer.Config.SmtpPassword, ct);
        return mailer with { Config = new MailerConfig(SmtpPassword: encryptedText) };
    }

    public async Task<Mailer> WithDecryptedPassword(Mailer mailer, CancellationToken ct)
    {
        string decryptedText = await cryptography.DecryptText(mailer.Config.SmtpPassword, ct);
        return mailer with { Config = new MailerConfig(SmtpPassword: decryptedText) };
    }
}