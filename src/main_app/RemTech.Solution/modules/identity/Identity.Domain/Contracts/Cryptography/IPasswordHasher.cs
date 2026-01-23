using Identity.Domain.Accounts.Models;

namespace Identity.Domain.Contracts.Cryptography;

public interface IPasswordHasher
{
	public AccountPassword Hash(AccountPassword password);
	public bool Verify(string input, AccountPassword hashed);
}
