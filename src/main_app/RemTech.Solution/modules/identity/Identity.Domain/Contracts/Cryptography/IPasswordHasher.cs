using Identity.Domain.Accounts.Models;

namespace Identity.Domain.Contracts.Cryptography;

public interface IPasswordHasher
{
	AccountPassword Hash(AccountPassword password, CancellationToken ct = default);
	bool Verify(string input, AccountPassword hashed, CancellationToken ct = default);
}
