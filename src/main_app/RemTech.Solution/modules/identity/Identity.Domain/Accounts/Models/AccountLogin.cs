using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed record AccountLogin
{
	private AccountLogin(string value)
	{
		Value = value;
	}

	public string Value { get; }

	public static Result<AccountLogin> Create(string value) =>
		string.IsNullOrWhiteSpace(value) ? Error.Validation("Логин не может быть пустым.") : new AccountLogin(value);
}
