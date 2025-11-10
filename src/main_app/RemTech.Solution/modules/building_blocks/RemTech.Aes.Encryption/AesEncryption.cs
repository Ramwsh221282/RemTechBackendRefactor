namespace RemTech.Aes.Encryption;

public sealed class AesEncryption(string keyText)
{
    private readonly AesEncryptionEngine _engine = new(keyText);

    public async Task<string> EncryptAsync(string plainText)
    {
        using AesScope scope = _engine.ProvideEncryptor();
        using AesStream stream = scope.ProvideStream();
        return await stream.WorkWithAsync(plainText);
    }

    public async Task<string> DecryptAsync(string encryptedText)
    {
        try
        {
            using AesScope scope = _engine.ProvideDecryptor();
            using AesStream stream = scope.ProvideStream();
            return await stream.WorkWithAsync(encryptedText);
        }
        catch (FormatException format)
        {
            return string.Empty;
        }
    }
}