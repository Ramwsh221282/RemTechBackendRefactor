using Users.Module.Features.UserPasswordRecovering.Core;

namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal interface IUserRecoveringMethod
{
    Task<PasswordResetMessageDetails> Invoke();
}
