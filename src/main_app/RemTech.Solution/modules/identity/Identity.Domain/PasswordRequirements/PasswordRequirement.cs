using Identity.Domain.Accounts.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Domain.PasswordRequirements;

public sealed class PasswordRequirement : IAccountPasswordRequirement
{
	private readonly List<IAccountPasswordRequirement> _requirements = [];

	public Result<Unit> Satisfies(AccountPassword password)
	{
		List<string> errors = [];
		foreach (IAccountPasswordRequirement requirement in _requirements)
		{
			Result<Unit> validation = requirement.Satisfies(password);
			if (validation.IsFailure)
				errors.Add(validation.Error.Message);
		}
		return errors.Count == 0 ? Unit.Value : Error.Validation(string.Join(", ", errors));
	}

	public PasswordRequirement Use(IAccountPasswordRequirement requirement)
	{
		_requirements.Add(requirement);
		return this;
	}

	public PasswordRequirement Use(IEnumerable<IAccountPasswordRequirement> requirements)
	{
		_requirements.AddRange(requirements);
		return this;
	}
}
