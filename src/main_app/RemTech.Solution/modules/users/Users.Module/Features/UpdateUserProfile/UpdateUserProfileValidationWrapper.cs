using RemTech.Core.Shared.Cqrs;
using Users.Module.CommonAbstractions;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed class UpdateUserProfileValidationWrapper(
    ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult> origin
) : ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    public Task<UpdateUserProfileResult> Handle(
        UpdateUserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UpdateUserDetails updateUserDetails = command.UpdateUserDetails;
        ValidateEmailIfProvided(updateUserDetails);
        return origin.Handle(command, ct);
    }

    private void ValidateEmailIfProvided(UpdateUserDetails details)
    {
        if (!string.IsNullOrWhiteSpace(details.NewUserEmail))
            new EmailValidation().ValidateEmail(details.NewUserEmail);
    }
}
