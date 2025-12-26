using Mailers.Application.Configs;
using Mailers.Core.MailersModule;
using Microsoft.Extensions.Options;
using RemTech.Aes.Encryption;

namespace Mailers.Application.Features.Encryptions;

public static class MailerSmtpPasswordEncryption
{
    extension(IOptions<MailersEncryptOptions> options)
    {
        public async Task<Result<SmtpPassword>> Encrypted(string password)
        {
            string key = options.Value.Key;
            return string.IsNullOrWhiteSpace(key)
                ? Error.Application("Не указан ключ для шифрования SMTP паролей.")
                : await new AesEncryption(key).Encrypt(password);
        }

        public async Task<Result<string>> Decrypted(string password)
        {
            string key = options.Value.Key;
            return string.IsNullOrWhiteSpace(key) ?
                Error.Application("Не указан ключ для шифрования SMTP паролей.") :
                await new AesEncryption(key).DecryptAsync(password);
        }

        public async Task<Result<string>> Decrypted(SmtpPassword password)
        {
            return await options.Decrypted(password.Value);
        }

        public async Task<Result<string>> Decrypted(Mailer mailer)
        {
            return await options.Decrypted(mailer.Password);
        }
    }
    
    extension(AesEncryption encryption)
    {
        public async Task<Result<SmtpPassword>> Encrypt(string password)
        {
            Result<SmtpPassword> passwordRes = SmtpPassword.Construct(password);
            return passwordRes.IsFailure 
                ? passwordRes.Error 
                : SmtpPassword.Construct(await encryption.EncryptAsync(passwordRes.Value.Value));
        }
    }

    extension(Mailer mailer)
    {
        public Task<Result<SmtpPassword>> EncryptPassword(IOptions<MailersEncryptOptions> options)
        {
            string password = mailer.Password.Value;
            return options.Encrypted(password);
        }

        public async Task<Result<Mailer>> WithEncryptedPassword(IOptions<MailersEncryptOptions> options)
        {
            string nonEncrypted = mailer.Password.Value;
            Result<SmtpPassword> password = await options.Encrypted(nonEncrypted);
            if (password.IsFailure) return password.Error;
            return mailer with { Password = password };
        }
    }
}