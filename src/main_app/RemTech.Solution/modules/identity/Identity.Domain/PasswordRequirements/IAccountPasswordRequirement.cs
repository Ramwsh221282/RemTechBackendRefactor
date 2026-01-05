using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public interface IAccountPasswordRequirement
{
    Result<Unit> Satisfies(AccountPassword password);
}