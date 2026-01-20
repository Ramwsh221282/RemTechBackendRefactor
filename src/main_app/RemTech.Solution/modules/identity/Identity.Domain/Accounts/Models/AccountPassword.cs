using Identity.Domain.Contracts.Cryptography;
using Identity.Domain.PasswordRequirements;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public sealed record AccountPassword
{
    public string Value { get; }

    private AccountPassword(string value) => Value = value;

    public static Result<AccountPassword> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Пароль не может быть пустым.");
        return new AccountPassword(value);
    }

    public AccountPassword HashBy(IPasswordHasher hasher)
    {
        return hasher.Hash(this);
    }

    public bool Verify(string input, IPasswordHasher hasher)
    {
        return hasher.Verify(input, this);
    }

    public Result<Unit> Satisfies(IAccountPasswordRequirement requirement)
    {
        return requirement.Satisfies(this);
    }
}
