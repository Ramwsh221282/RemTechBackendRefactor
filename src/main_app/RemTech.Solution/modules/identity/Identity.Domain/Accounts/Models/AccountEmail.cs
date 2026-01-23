using System.Text.RegularExpressions;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed record AccountEmail
{
	private static readonly Regex EmailRegex = new(
		@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
		RegexOptions.Compiled
	);
	private const int MaxEmailLength = 256;
	public string Value { get; }

	private AccountEmail(string value) => Value = value;

	public static Result<AccountEmail> Create(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
			return Error.Validation("Email не может быть пустым.");
		if (value.Length > MaxEmailLength)
			return Error.Validation($"Email не может быть длиннее {MaxEmailLength} символов.");
		if (!EmailRegex.IsMatch(value))
			return Error.InvalidFormat($"Email: {value} некоррекнтого формата.");
		return new AccountEmail(value);
	}
}
