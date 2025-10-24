using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Identity.Adapter.Jwt.Security;

public sealed class RsaSecurityKeyPairStorage(RedisCache cache) : IRsaSecurityTokenPairStorage
{
    private const string PublicKeyName = "RSA_PUBLIC_KEY";
    private const string PrivateKeyName = "RSA_PRIVATE_KEY";
    private readonly IDatabase _database = cache.Database;
    private bool _generatedFromStart;

    public async Task Generate()
    {
        if (_generatedFromStart)
            return;

        await RemoveKey(PublicKeyName);
        await RemoveKey(PrivateKeyName);

        using var rsa = RSA.Create(2048);
        var privateKey = rsa.ExportPkcs8PrivateKey();
        var publicKey = rsa.ExportSubjectPublicKeyInfo();

        await StoreKey(PrivateKeyName, privateKey);
        await StoreKey(PublicKeyName, publicKey);
        _generatedFromStart = true;
    }

    public async Task<RsaSecurityKey> Get()
    {
        RSA rsa = RSA.Create(2048);
        rsa = await ImportPrivateKey(rsa);
        return new RsaSecurityKey(rsa);
    }

    private async Task<RSA> ImportPrivateKey(RSA rsa)
    {
        if (!await DoesKeyExist(PublicKeyName))
            throw new KeyNotFoundException($"Key {PublicKeyName} does not exist.");

        ReadOnlyMemory<byte> key = await GetKeyBytes(PrivateKeyName);
        rsa.ImportPkcs8PrivateKey(key.Span, out _);
        return rsa;
    }

    private async Task<RSA> ImportPublicKey(RSA rsa)
    {
        if (!await DoesKeyExist(PublicKeyName))
            throw new KeyNotFoundException($"Key {PublicKeyName} does not exist.");

        ReadOnlyMemory<byte> key = await GetKeyBytes(PublicKeyName);
        rsa.ImportSubjectPublicKeyInfo(key.Span, out _);
        return rsa;
    }

    private async Task<ReadOnlyMemory<byte>> GetKeyBytes(string key)
    {
        var fromRedis = await _database.StringGetAsync(key);
        if (fromRedis.HasValue == false)
            throw new ApplicationException("Rsa public key was not generated.");

        byte[]? keyBytes = fromRedis;
        if (keyBytes == null)
            throw new ApplicationException("Rsa public key was not generated.");

        if (keyBytes.Length == 0)
            throw new ApplicationException("Rsa public key was not generated.");

        return keyBytes;
    }

    private async Task<bool> DoesKeyExist(string key)
    {
        return await _database.KeyExistsAsync(key);
    }

    private async Task RemoveKey(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    private async Task StoreKey(string keyName, byte[] keyBytes)
    {
        await _database.StringSetAsync(keyName, keyBytes, flags: CommandFlags.FireAndForget);
    }
}
