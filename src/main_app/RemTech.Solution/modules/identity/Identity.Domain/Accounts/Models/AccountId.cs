using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.Accounts.Models;

public readonly record struct AccountId
{
    public Guid Value { get; }

    public AccountId() => Value = Guid.NewGuid();

    private AccountId(Guid value) => Value = value;

    public static AccountId New() => new(Guid.NewGuid());

    public static Result<AccountId> Create(Guid value)
    {
        return value == Guid.Empty
            ? Error.Validation("Идентификатор учетной записи не может быть пустым.")
            : new AccountId(value);
    }
}