using Identity.Domain.Contracts;
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
        if (string.IsNullOrWhiteSpace(value)) return Error.Validation("Пароль не может быть пустым.");
        return new AccountPassword(value);
    }

    public AccountPassword HashBy(IPasswordHasher hasher, CancellationToken ct = default)
    {
        return hasher.Hash(this, ct);
    }

    public bool Verify(string input, IPasswordHasher hasher, CancellationToken ct = default)
    {
        return hasher.Verify(input, this, ct);
    }
    
    public Result<Unit> Satisfies(IAccountPasswordRequirement requirement)
    {
        return requirement.Satisfies(this);
    }
}