using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class MinLengthPasswordRequirement : IAccountPasswordRequirement
{
    private const int Length = 8;
    
    public Result<Unit> Satisfies(AccountPassword password)
    {
        return password.Value.Length < Length
            ? Error.Validation($"Пароль должен быть длиннее {Length} символов.")
            : Unit.Value;
    }
}