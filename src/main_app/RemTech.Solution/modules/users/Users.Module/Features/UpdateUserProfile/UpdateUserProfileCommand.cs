using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Models;

namespace Users.Module.Features.UpdateUserProfile;

internal sealed record UpdateUserProfileCommand(
    UserJwt Jwt,
    PreviousUserDetails PreviousDetails,
    UpdateUserDetails UpdateUserDetails
) : ICommand;
