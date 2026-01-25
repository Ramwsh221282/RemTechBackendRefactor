using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.PasswordRequirements;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed record AccountPassword
{
	private AccountPassword(string value) => Value = value;

	public string Value { get; }

	public static Result<AccountPassword> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? (Result<AccountPassword>)Error.Validation("Пароль не может быть пустым.")
			: (Result<AccountPassword>)new AccountPassword(value);
	}

	public AccountPassword HashBy(IPasswordHasher hasher) => hasher.Hash(this);

	public bool Verify(string input, IPasswordHasher hasher) => hasher.Verify(input, this);

	public Result<Unit> Satisfies(IAccountPasswordRequirement requirement) => requirement.Satisfies(this);
}
