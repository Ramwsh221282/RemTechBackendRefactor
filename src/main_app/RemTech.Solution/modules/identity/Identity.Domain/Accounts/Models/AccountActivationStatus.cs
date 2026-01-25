namespace Identity.Domain.Accounts.Models;

public readonly record struct AccountActivationStatus
{
	public AccountActivationStatus() => Value = false;

	public bool Value { get; private init; }

	public static AccountActivationStatus NotActivated() => new();

	public static AccountActivationStatus Activated() => new() { Value = true };

	public static AccountActivationStatus Create(bool value) => new() { Value = value };

	public bool IsActivated() => Value;
}
