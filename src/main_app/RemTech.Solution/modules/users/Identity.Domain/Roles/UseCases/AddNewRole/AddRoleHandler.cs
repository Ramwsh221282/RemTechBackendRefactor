using FluentValidation;
using FluentValidation.Results;
using Identity.Domain.Roles.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;
using RemTech.Core.Shared.Validation;

namespace Identity.Domain.Roles.UseCases.AddNewRole;

public sealed class AddRoleHandler(
    IDomainEventsDispatcher dispatcher,
    IValidator<AddRoleCommand> validator
) : ICommandHandler<AddRoleCommand, Status<IdentityRole>>
{
    public async Task<Status<IdentityRole>> Handle(
        AddRoleCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult? validation = await validator.ValidateAsync(command, ct);
        if (validation.IsValid == false)
            return validation.ValidationError();

        RoleName name = RoleName.Create(command.RoleName);
        IdentityRole identityRole = IdentityRole.Create(name);

        Status processing = await identityRole.PublishEvents(dispatcher, ct);
        return processing.IsFailure ? processing.Error : identityRole;
    }
}
