using FluentValidation;
using Identity.Domain.Roles.ValueObjects;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Roles.UseCases.AddNewRole;

public sealed class AddRoleValidator : AbstractValidator<AddRoleCommand>
{
    public AddRoleValidator()
    {
        RuleFor(c => c.RoleName).MustBeValid(RoleName.Create);
    }
}
