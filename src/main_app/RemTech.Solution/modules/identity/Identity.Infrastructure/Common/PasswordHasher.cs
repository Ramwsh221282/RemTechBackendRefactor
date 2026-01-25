using BCrypt.Net;
using Identity.Domain.Accounts.Models;
using Identity.Domain.Contracts.Cryptography;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Common;

public sealed class PasswordHasher(IOptions<BcryptWorkFactorOptions> options) : IPasswordHasher
{
    private int WorkFactor { get; } = options.Value.WorkFactor;

    public AccountPassword Hash(AccountPassword password)
    {
        string hashed = BCrypt.Net.BCrypt.EnhancedHashPassword(password.Value, HashType.SHA512, WorkFactor);
        return AccountPassword.Create(hashed);
    }

    public bool Verify(string input, AccountPassword hashed) =>
        BCrypt.Net.BCrypt.EnhancedVerify(input, hashed.Value, HashType.SHA512);
}
