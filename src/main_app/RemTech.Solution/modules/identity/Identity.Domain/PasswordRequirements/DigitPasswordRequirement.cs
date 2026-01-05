using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class DigitPasswordRequirement : IAccountPasswordRequirement
{
    public Result<Unit> Satisfies(AccountPassword password)
    {
        return !password.Value.Any(char.IsDigit)
            ? Error.Validation("Пароль должен содержать хотя бы одну цифру.")
            : Unit.Value;
    }
}