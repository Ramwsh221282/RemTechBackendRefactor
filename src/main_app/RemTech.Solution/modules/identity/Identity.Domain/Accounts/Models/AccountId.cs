using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public readonly record struct AccountId
{
	public AccountId() => Value = Guid.NewGuid();

	private AccountId(Guid value) => Value = value;

	public Guid Value { get; }

	public static AccountId New() => new(Guid.NewGuid());

	public static Result<AccountId> Create(Guid value) =>
		value == Guid.Empty
			? Error.Validation("Идентификатор учетной записи не может быть пустым.")
			: new AccountId(value);
}
