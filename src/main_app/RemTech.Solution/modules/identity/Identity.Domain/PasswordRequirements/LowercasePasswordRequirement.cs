using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class LowercasePasswordRequirement : IAccountPasswordRequirement
{
    public Result<Unit> Satisfies(AccountPassword password)
    {
        return !password.Value.Any(char.IsLower)
            ? Error.Validation("Пароль должен содержать хотя бы одну строчную букву.")
            : Unit.Value;
    }
}