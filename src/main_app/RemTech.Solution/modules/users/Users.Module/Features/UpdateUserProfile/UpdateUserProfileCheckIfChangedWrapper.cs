using Shared.Infrastructure.Module.Cqrs;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed class UpdateUserProfileCheckIfChangedWrapper(
    ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult> origin
) : ICommandHandler<UpdateUserProfileCommand, UpdateUserProfileResult>
{
    public Task<UpdateUserProfileResult> Handle(
        UpdateUserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UpdateUserDetails updatedDetails = command.UpdateUserDetails;
        bool anyChanged =
            IsEmailChanged(updatedDetails)
            || IsNameChanged(updatedDetails)
            || IsRoleChanged(updatedDetails)
            || IsPasswordUpdateRequested(updatedDetails);
        return anyChanged ? origin.Handle(command, ct) : Task.FromResult(UnchangedResult(command));
    }

    private UpdateUserProfileResult UnchangedResult(UpdateUserProfileCommand command)
    {
        PreviousUserDetails details = command.PreviousDetails;
        return new UpdateUserProfileResult(
            details.UserId,
            details.UserEmail,
            details.UserName,
            details.UserRole
        );
    }

    private bool IsEmailChanged(UpdateUserDetails updated) =>
        !string.IsNullOrWhiteSpace(updated.NewUserEmail);

    private bool IsNameChanged(UpdateUserDetails updated) =>
        !string.IsNullOrWhiteSpace(updated.NewUserName);

    private bool IsRoleChanged(UpdateUserDetails updated) =>
        !string.IsNullOrWhiteSpace(updated.NewUserRole);

    private bool IsPasswordUpdateRequested(UpdateUserDetails updated) =>
        updated.IsPasswordUpdateRequired;
}
