using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class SpecialCharacterPasswordRequirement : IAccountPasswordRequirement
{
    private static readonly char[] SpecialChars = "!@#$%^&*()_+-=[]{};:'\",.<>/?\\|`~".ToCharArray();

    public Result<Unit> Satisfies(AccountPassword password)
    {
        return !password.Value.Any(c => SpecialChars.Contains(c))
            ? (Result<Unit>)Error.Validation("Пароль должен содержать хотя бы один спецсимвол.")
            : (Result<Unit>)Unit.Value;
    }
}
