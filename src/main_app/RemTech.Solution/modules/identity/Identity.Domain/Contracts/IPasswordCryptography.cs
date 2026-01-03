using Identity.Domain.Accounts.Models;

namespace Identity.Domain.Contracts;

public interface IPasswordCryptography
{
    Task<AccountPassword> Encrypt(AccountPassword password, CancellationToken ct = default);
    Task<AccountPassword> Decrypt(AccountPassword password, CancellationToken ct = default);
}