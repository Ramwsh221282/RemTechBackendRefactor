using Identity.Domain.Contracts;
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

    public async Task<AccountPassword> Encrypt(IPasswordCryptography cryptography, CancellationToken ct = default)
    {
        return await cryptography.Encrypt(this, ct);
    }

    public async Task<AccountPassword> Decrypt(IPasswordCryptography cryptography, CancellationToken ct = default)
    {
        return await cryptography.Decrypt(this, ct);
    }
    
    public Result<Unit> Satisfies(IAccountPasswordRequirement requirement)
    {
        return requirement.Satisfies(this);
    }
}