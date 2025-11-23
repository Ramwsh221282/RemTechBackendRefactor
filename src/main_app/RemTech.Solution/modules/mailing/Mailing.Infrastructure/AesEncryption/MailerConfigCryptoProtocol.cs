using System.Security.Cryptography;
using Mailing.Core.Mailers;
using Mailing.Core.Mailers.Protocols;
using Microsoft.Extensions.Options;

namespace Mailing.Infrastructure.AesEncryption;

public sealed record MailerConfigCryptoProtocol(
    IOptions<AesEncryptionOptions> Options) 
    : EncryptMailerSmtpPasswordProtocol, 
        DecryptMailerSmtpPasswordProtocol
{
    private const int BytesLength = 16;
    
    public async Task<Mailer> WithEncryptedPassword(Mailer mailer, CancellationToken ct)
    {
        using Aes aes = Aes.Create();
        aes.Key = Options.Value.KeyAsBytes();
        aes.IV = Options.Value.IV4AsBytes();
        using ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
        await using MemoryStream ms = new();
        await ms.WriteAsync(aes.IV, ct);
        await using CryptoStream cs = new(ms, transform, CryptoStreamMode.Write);
        await using StreamWriter sw = new(cs);
        await sw.WriteAsync(mailer.Config.SmtpPassword);
        return mailer with { Config = new MailerConfig(SmtpPassword: Convert.ToBase64String(ms.ToArray())) };
    }

    public async Task<Mailer> WithDecryptedPassword(Mailer mailer, CancellationToken ct)
    {
        byte[] base64Text = Convert.FromBase64String(mailer.Config.SmtpPassword);
        byte[] iv = new byte[BytesLength];
        Array.Copy(base64Text, iv, BytesLength);
        using Aes aes = Aes.Create();
        aes.Key = Options.Value.KeyAsBytes();
        aes.IV = iv;
        using ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
        await using MemoryStream ms = new(base64Text, BytesLength, base64Text.Length - BytesLength);
        await using CryptoStream cs = new(ms, transform, CryptoStreamMode.Read);
        using StreamReader sr = new(cs);
        return mailer with { Config = new MailerConfig(SmtpPassword: await sr.ReadToEndAsync(ct)) };
    }
}